using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Commands
{
    [IrcCommand("help")]
    public class HelpCommand : IrcCommandProcessor
    {
        public override void Process(IrcCommand command)
        {
            base.Process(command);

            var list = new IrcCommandProcessorList();
            var commands = list.Processors.Select(p => p.Key).ToArray();
            var commandList = string.Join(", ", commands);

            SendNotice(
                string.Format(
                    "{0} supports the following commands: {1}", 
                    command.Bot.Configuration.NickName,
                    commandList
                )
            );

        }
    }
}
