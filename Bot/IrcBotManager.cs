using System;
using System.Configuration;
using Bot.Tasks;
using Topshelf;

namespace Bot
{
    /// <summary>
    /// Responsible for running IRC bots based on system configuration.
    /// </summary>
    public class IrcBotManager : ServiceControl
    {
        private readonly IrcBot bot;
        private readonly IrcBotConfiguration configuration;

        private readonly string teamCityFeedUri;
        private readonly string productionElb;
        private readonly string uatElb;
        private readonly string statusPageBucketName;

        public IrcBotManager()
        {
            this.configuration = CreateConfiguration();
            this.bot = new IrcBot(this.configuration);

            this.teamCityFeedUri = ConfigurationManager.AppSettings["feed"];
            this.productionElb = ConfigurationManager.AppSettings["productionElbName"];
            this.uatElb = ConfigurationManager.AppSettings["uatElbName"];
            this.statusPageBucketName = ConfigurationManager.AppSettings["statusPageBucket"];

            // report build status
            if (!string.IsNullOrWhiteSpace(this.teamCityFeedUri))
                this.bot.AddTask(new IrcTeamCityBuildStatusTask(
                    new Uri(this.teamCityFeedUri),
                    "Insight TeamCity Build Status Task"
                ));

            if (!string.IsNullOrWhiteSpace(productionElb))
            {
                // report production aws elb status
                this.bot.AddTask(new IrcElbStatusTask(
                    productionElb,
                    "Insight Production ELB Status Task"
                ));

                // reboot unhealthy ec2 instances on production elb
                this.bot.AddTask(new InsightInstanceMonitor(
                    productionElb,
                    "Insight Production Unhealthy EC2 Instance Monitor Task"
                ));
            }

            if (!string.IsNullOrWhiteSpace(uatElb))
            {
                // report uat aws elb status
                this.bot.AddTask(new IrcElbStatusTask(
                    uatElb,
                    "Insight UAT ELB Status Task"
                ));

                // reboot unhealthy ec2 instances on uat elb
                this.bot.AddTask(new InsightInstanceMonitor(
                    uatElb,
                    "Insight UAT Unhealthy EC2 Instance Monitor Task"
                ));
            }

            // upload aws elb status page to s3
            if (!string.IsNullOrEmpty(productionElb) && !string.IsNullOrEmpty(this.statusPageBucketName))
                this.bot.AddTask(new InsightInstanceUrlUploadTask(productionElb, this.statusPageBucketName));

            //this.bot.AddTask(new InsightInstanceStatsMonitorTask("healthcheck/stats"));
        }

        private IrcBotConfiguration CreateConfiguration()
        {
            return new IrcBotConfiguration {
                NickName = ConfigurationManager.AppSettings["nick"],
                HostName = ConfigurationManager.AppSettings["host"],
                Port = int.Parse(ConfigurationManager.AppSettings["port"]),
                Channel = ConfigurationManager.AppSettings["channel"]
            };
        }

        public bool Start(HostControl hostControl)
        {
            this.bot.Start(hostControl);
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            this.bot.Stop();
            this.bot.Dispose();
            return true;
        }

    }
}
