using IrcDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Commands
{
    public class QuitCommand : IIrcCommandProcessor
    {
        public void Process(IrcCommand command)
        {
            command.Client.LocalUser.SendMessage(
                command.Target,
                string.Format("Ok {0}, I'm outta here!", command.Source.Name)
            );

            command.Client.Quit(
                string.Format(
                    "{0} made me do it", 
                    command.Source.Name
                )
            );

            command.Client.Disconnect();
        }
    }
}
