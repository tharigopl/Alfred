using System;
using System.Configuration;
using Bot;
using Bot.Tasks;

namespace BotConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = CreateConfiguration();
            var bot = new IrcBot(config);
            var uriString = ConfigurationManager.AppSettings["feed"];
            var productionElb = ConfigurationManager.AppSettings["productionElbName"];
            var uatElb = ConfigurationManager.AppSettings["uatElbName"];
            var statusPageBucket = ConfigurationManager.AppSettings["statusPageBucket"];

            // report build status
            if (!string.IsNullOrWhiteSpace(uriString))
                bot.AddTask(new IrcTeamCityBuildStatusTask(new Uri(uriString)));

            // report aws elb status
            if (!string.IsNullOrWhiteSpace(productionElb))
                bot.AddTask(new IrcElbStatusTask(productionElb));

            if (!string.IsNullOrWhiteSpace(uatElb))
                bot.AddTask(new IrcElbStatusTask(uatElb));

            // upload aws elb status page to s3
            if (!string.IsNullOrEmpty(productionElb) && !string.IsNullOrEmpty(statusPageBucket))
                bot.AddTask(new InsightInstanceUrlUploadTask(productionElb, statusPageBucket));

            // GO!
            bot.Run().Wait();
        }

        static IrcBotConfiguration CreateConfiguration()
        {
            return new IrcBotConfiguration {
                NickName = ConfigurationManager.AppSettings["nick"],
                HostName = ConfigurationManager.AppSettings["host"],
                Port = int.Parse(ConfigurationManager.AppSettings["port"]),
                Channel = ConfigurationManager.AppSettings["channel"]
            };
        }
    }
}
