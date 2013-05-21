using System;
using System.Linq;
using System.Net;
using Bot.Infrastructure.AWS;
using Bot.Infrastructure.TradeStation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bot.Tasks
{
    public class InsightInstanceStatsMonitorTask : IrcTask
    {

        private readonly string endpoint;
        private readonly ELB elb;
        private readonly EC2 ec2;

        public InsightInstanceStatsMonitorTask(string endpoint, string name = null)
            : base(name ?? "Insight Instance Stats Monitor Task")
        {
            this.elb = new ELB();
            this.ec2 = new EC2();
            this.SleepTime = TimeSpan.FromSeconds(30);
            this.endpoint = endpoint ?? string.Empty;
        }

        protected override void Execute()
        {
            var instanceIds = this.elb.Instances("MonexUAT").Select(i => i.InstanceId);
            var descriptions = this.ec2.InstanceDescriptions(instanceIds);

            foreach (var description in descriptions)
            {
                var stats = GetStatsForInstance(description.PublicDnsName);

                SendMessage(
                    string.Format(
                        "WebAPI: {0} Tahiti: {1} Tahiti-Timer: {2}",
                        stats.WebApiStatus,
                        stats.TahitiStatus,
                        stats.TahitiTimer
                    )
                );
            }
        }

        private WebApiStats GetStatsForInstance(string instanceBaseUrl)
        {
            var client = new WebClient();
            var url = string.Format(
                    @"http://{0}/{1}",
                    instanceBaseUrl,
                    this.endpoint
                );
            var json = client.DownloadString(url);
            var stats = JsonConvert.DeserializeObject<WebApiStats>(json);

            return stats;

            
        }
    }
}
