using IrcDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Commands
{
    public class EchoCommand : IIrcCommandProcessor
    {
        public void Process(IrcCommand command)
        {
            var message = string.Join(" ", command.Parameters);
            command.Client.LocalUser.SendMessage(command.Target, message);
        }
    }
}
