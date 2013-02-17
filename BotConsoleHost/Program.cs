using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            var uriString = ConfigurationSettings.AppSettings["feed"];
            bot.AddTask(new IrcTeamCityBuildStatusTask(new Uri(uriString)));
            bot.Run().Wait();
        }

        static IrcBotConfiguration CreateConfiguration()
        {
            return new IrcBotConfiguration {
                NickName = ConfigurationSettings.AppSettings["nick"],
                HostName = ConfigurationSettings.AppSettings["host"],
                Port = int.Parse(ConfigurationSettings.AppSettings["port"]),
                Channel = ConfigurationSettings.AppSettings["channel"]
            };
        }
    }
}
