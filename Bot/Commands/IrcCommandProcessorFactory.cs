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

        public IIrcCommandProcessor CreateByCommand(IrcCommand channelCommand)
        {
            if (!string.IsNullOrEmpty(channelCommand.Name) && this.commandMap.ContainsKey(channelCommand.Name))
            {
                var type = this.commandMap[channelCommand.Name];
                var instance = Activator.CreateInstance(type);
                return (IIrcCommandProcessor)instance;
            }

            return new UnsupportedCommand();
        }


    }
}
