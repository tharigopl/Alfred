using System;
using System.Configuration;
using Bot;
using Bot.Tasks;
using Topshelf;
using Topshelf.Runtime;

namespace BotConsoleHost
{
    class Program
    {
        public static void Main()
        {
            HostFactory.Run(hc => {
                hc.Service<IrcBotManager>(sc => {
                    sc.ConstructUsing(name => new IrcBotManager());
                    sc.WhenStarted(m => m.Start());
                    sc.WhenStopped(m => m.Stop());
                });

                hc.StartAutomaticallyDelayed();
                hc.RunAsLocalSystem();
                //hc.UseNLog();

                hc.EnableServiceRecovery(rc => {
                    rc.RestartService(1);
                });

                hc.SetDescription("Alfred the IRC bot.");
                hc.SetDisplayName("Alfred");
                hc.SetServiceName("Alfred");
            });
        }

    }
}
