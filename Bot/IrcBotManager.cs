﻿using System;
using System.Configuration;
using Bot.Tasks;

namespace Bot
{
    /// <summary>
    /// Responsible for running IRC bots based on system configuration.
    /// </summary>
    public class IrcBotManager
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
                this.bot.AddTask(new IrcTeamCityBuildStatusTask(new Uri(this.teamCityFeedUri)));

            // report aws elb status
            if (!string.IsNullOrWhiteSpace(productionElb))
                this.bot.AddTask(new IrcElbStatusTask(productionElb));

            if (!string.IsNullOrWhiteSpace(uatElb))
                this.bot.AddTask(new IrcElbStatusTask(uatElb));

            // upload aws elb status page to s3
            if (!string.IsNullOrEmpty(productionElb) && !string.IsNullOrEmpty(this.statusPageBucketName))
                this.bot.AddTask(new InsightInstanceUrlUploadTask(productionElb, this.statusPageBucketName));

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

        public void Start()
        {
            this.bot.Start();
        }

        public void Stop()
        {
            this.bot.Stop();
        }
    }
}
