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
        private readonly Dictionary<string, Type> commandMap;

        public IrcCommandProcessorFactory(Dictionary<string, Type> processors)
        {
            this.commandMap = processors;
        }

        public IIrcCommandProcessor CreateByCommand(IrcCommand command)
        {
            if (!string.IsNullOrEmpty(command.Name) && this.commandMap.ContainsKey(command.Name))
            {
                var type = this.commandMap[command.Name];
                var instance = Activator.CreateInstance(type);
                return (IIrcCommandProcessor)instance;
            }

            return new UnsupportedCommand();
        }


    }
}
