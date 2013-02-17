using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Commands
{
    public class IrcCommandProcessorFactory
    {
        public IIrcCommandProcessor GetByCommand(IrcCommand command)
        {
            switch (command.Name) {
                case "echo": return new EchoCommand();
                case "quit": return new QuitCommand();
                case "status": return new BuildStatusCommand();
                default: return new UnsupportedCommand();
            }
        }
    }
}
