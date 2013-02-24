using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Commands
{
    public class IrcCommandProcessorFactory
    {
        private readonly Dictionary<string, Type> processors;

        public IrcCommandProcessorFactory()
        {
            var list = new IrcCommandProcessorList();
            this.processors = list.Processors;
        }

        public IIrcCommandProcessor GetByCommand(IrcCommand command)
        {
            if (this.processors.ContainsKey(command.Name))
            {
                var type = this.processors[command.Name];
                var instance = Activator.CreateInstance(type);
                return (IIrcCommandProcessor)instance;
            }

            return new UnsupportedCommand();
        }

    }
}
