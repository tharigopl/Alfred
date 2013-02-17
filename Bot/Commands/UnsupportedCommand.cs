using IrcDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Commands
{
    public class UnsupportedCommand : IIrcCommandProcessor
    {
        public void Process(IrcCommand command)
        {
            command.Client.LocalUser.SendMessage(
                command.Target, 
                string.Format(
                    "Sorry {0}, I don't understand that command.",
                    command.Source.Name
                )
            );
        }
    }
}
