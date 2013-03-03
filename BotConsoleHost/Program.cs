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
            //bot.AddTask(new IrcTeamCityBuildStatusTask(new Uri(uriString)));
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
