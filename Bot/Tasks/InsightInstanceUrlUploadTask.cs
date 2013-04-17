using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.EC2.Model;
using Bot.Extensions;
using Bot.Infrastructure;
using Bot.Infrastructure.AWS;

namespace Bot.Tasks
{
    public class InsightInstanceUrlUploadTask : IrcTask
    {
        private List<RunningInstance> descriptions;
        private List<Amazon.ElasticLoadBalancing.Model.InstanceState> states;
        private Dictionary<string, string> stateMap;
        private string fileContents;

        private readonly ELB elb;
        private readonly EC2 ec2;
        private readonly S3 s3;

        private readonly string loadBalancerName;
        private readonly string bucketName;

        public InsightInstanceUrlUploadTask(string loadBalancerName, string bucketName)
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
            FormatUrls();
            UploadUrls();
        }
        
        private void FormatUrls()
        {
            var sb = new StringBuilder();


            sb.Append(@"
                <html><head>
                    <meta http-equiv=""refresh"" content=""15"" >
                    <meta name=""viewport"" content=""width=device-width; initial-scale=1.0; maximum-scale=1.0; user-scalable=0;"" />
                    <style>
                    body { font-family: 'Courier New'; font-size: 19px; }
            		p { padding-bottom: 10px; }
            		ul { list-style-type: none; margin: 0; padding: 0; width: 650px; }
            		li { display:inline-block; font-size: .75em; margin: 5px; border: 1px dashed Black; width: 100px; height: 100px; text-align: center; line-height: 100px; }
                    li.ok { background-color: Lime; }
                    li.warning { background-color: Yellow; }
                    li.error { background-color: Orange; }
                    li.danger { background-color: Red; }
            		a { text-decoration: none; color: Black; }
                    </style>
                </head><body>"
            );
            sb.AppendFormat(
                "<p>Last updated: {0} (UTC)</p>", 
                SystemTime.Now().ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")
            );

            var elbStates = ElbState.GetStates(loadBalancerName);
            var stateTimeMap = elbStates.ToDictionary(
                s => s.State.InstanceId, 
                s => SystemTime.Now() - s.TimeRemoved
            );

            sb.Append("<ul>");
            foreach (var description in this.descriptions)
            {
                var instanceStatus = this.stateMap[description.InstanceId];
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

                sb.AppendFormat(
                    @"<a href=""http://{0}/healthcheck""><li class=""{1}"">{2}</li></a>",
                    description.PublicDnsName,
                    status,
                    description.InstanceId
                );
            }

            sb.Append("</ul></body></html>");

            this.fileContents = sb.ToString();
        }

        private void UploadUrls()
        {
            this.s3.TryPut(
                bucketName,
                "index.html",
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
