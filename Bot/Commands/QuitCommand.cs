using IrcDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Bot.Commands.Attributes;

namespace Bot.Commands
{
    [IrcCommand("quit")]
    public class QuitCommand : IrcCommandProcessor
    {
        public override void Process(IrcCommand command)
        {
            base.Process(command);

            SendMessage(
                string.Format("Ok {0}, I'm outta here!", command.Source.Name)
            );

            Thread.Sleep(250);

            command.Client.Quit(
                string.Format(
                    "{0} made me do it", 
                    command.Source.Name
                )
            );

            Thread.Sleep(250);

            command.Client.Disconnect();
        }
    }
}
