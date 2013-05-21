using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.EC2.Model;
using Bot.Extensions;
using Bot.Infrastructure;
using Bot.Infrastructure.AWS;
using Bot.Infrastructure.TradeStation;
using Newtonsoft.Json;

namespace Bot.Tasks
{
    public class InsightInstanceUrlUploadTask : IrcTask
    {
        private const string STATS_ENDPOINT = "healthcheck/stats";

        private List<RunningInstance> descriptions;
        private List<Amazon.ElasticLoadBalancing.Model.InstanceState> states;
        private Dictionary<string, string> stateMap;
        private string fileContents;

        private readonly ELB elb;
        private readonly EC2 ec2;
        private readonly S3 s3;

        private readonly string loadBalancerName;
        private readonly string bucketName;

        public InsightInstanceUrlUploadTask(string loadBalancerName, string bucketName, string taskName = null)
            : base(taskName ?? "Insight Status Page Task")
        {
            loadBalancerName.Required("loadBalancerName");
            bucketName.Required("bucketName");

            this.loadBalancerName = loadBalancerName;
            this.bucketName = bucketName;

            this.elb = new ELB();
            this.ec2 = new EC2();
            this.s3 = new S3();
        }

        protected override void Execute()
        {
            GetInstanceStates();
            GetDescriptions();
            //FormatUrls();
            BuildJson();
            UploadJson();
        }
        
        private class InstanceStatus
        {
            public string tahitiStatus;
            public string tahitiTimer;
            public string webApiStatus;
            public string statusTimer;
            public string url { get; set; }
            public string publicDns { get; set; }
            public string status { get; set; }
            public string instanceId { get; set; }
            public string graylogUrl { get; set; }
        }

        private class InstanceStatusMessage
        {
            public string timestamp { get; set; }
            public List<InstanceStatus> instances { get; set; }

            public InstanceStatusMessage()
            {
                instances = new List<InstanceStatus>();
            }
        }

        private void BuildJson()
        {
            var statusMessage = new InstanceStatusMessage();
            var elbStates = ElbState.GetStates(loadBalancerName);
            var stateTimeMap = elbStates.ToDictionary(
                s => s.State.InstanceId, 
                s => SystemTime.Now - s.TimeRemoved
            );

            Parallel.ForEach(this.descriptions, description => {
                if (!this.stateMap.ContainsKey(description.InstanceId))
                    return;

                var instanceStatus = this.stateMap[description.InstanceId];
                var status = GetInstanceStatusCss(instanceStatus, stateTimeMap, description);
                var stats = GetStatsForInstance(description.PublicDnsName);
                var graylogUrl = GetGraylogUrl(description);

                statusMessage.instances.Add(
                    new InstanceStatus {
                        url = string.Format(
                            "http://{0}/healthcheck",
                            description.PublicDnsName
                        ),
                        graylogUrl = graylogUrl,
                        publicDns = description.PublicDnsName,
                        status = status,
                        instanceId = description.InstanceId,
                        webApiStatus = stats.WebApiStatus,
                        tahitiStatus = stats.TahitiStatus,
                        tahitiTimer = stats.TahitiTimer,
                        statusTimer = stats.StatsTime
                    }
                );

            });

            statusMessage.timestamp = SystemTime.UtcNow.ToString("o");

            this.fileContents = JsonConvert.SerializeObject(statusMessage);
        }

        private string GetGraylogUrl(RunningInstance description)
        {
            var graylogDns = ConfigurationManager.AppSettings["GraylogDns"];
            if (string.IsNullOrEmpty(graylogDns))
                throw new ConfigurationErrorsException("GraylogDns setting is missing from configuration.");

            var host = description.PrivateDnsName.Split('.')[0];

            return string.Format(
                "http://{0}/messages?filters%5Bseverity%5D=3&filters%5Bhost%5D={1}",
                graylogDns,
                host
            );
        }

        private static string GetInstanceStatusCss(string instanceStatus, Dictionary<string, TimeSpan> stateTimeMap, RunningInstance description)
        {
            var status = "ok";

            if (instanceStatus != "InService")
            {
                status = "warning";
                if (stateTimeMap.ContainsKey(description.InstanceId) && stateTimeMap[description.InstanceId].Minutes >= 10)
                {
                    status = "error";

                    if (stateTimeMap[description.InstanceId].Minutes >= 20)
                    {
                        status = "danger";
                    }
                }
            }
            return status;
        }

        private WebApiStats GetStatsForInstance(string instanceBaseUrl)
        {
            var url = string.Format(
                    @"http://{0}/{1}",
                    instanceBaseUrl,
                    STATS_ENDPOINT
            );

            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var client = new WebClientWithTimeout();
                var json = client.DownloadString(url);
                sw.Stop();
                var stats = JsonConvert.DeserializeObject<WebApiStats>(json);
                stats.StatsTime = sw.ElapsedMilliseconds.ToString();
                return stats;
            }
            catch (Exception ex)
            {
                return new WebApiStats {
                    StatsTime = "error"
                };
            }
        }

        private void UploadJson()
        {
            this.s3.TryPut(
                bucketName,
                "instances.json",
                this.fileContents,
                "text/html"
            );
        }

        private void GetDescriptions()
        {
            this.descriptions = this.ec2.InstanceDescriptions(states.Select(i => i.InstanceId));
        }

        private void GetInstanceStates()
        {
            this.states = this.elb.InstanceState(loadBalancerName);
            this.stateMap = states.ToDictionary(k => k.InstanceId, v => v.State);
        }
    }
}
