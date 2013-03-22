using IrcDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands.Attributes;

namespace Bot.Commands
{
    [IrcCommand("echo")]
    public class EchoCommand : IrcCommandProcessor
    {
        public override void Process(IrcCommand command)
        {
            base.Process(command);

            SendMessage(
                string.Join(" ", command.Parameters)
            );
        }
    }
}
