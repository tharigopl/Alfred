using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands.Attributes;

namespace Bot.Commands
{
    public class IrcHelpCommandProcessor<T> : IrcCommandProcessor where T: IrcCommandAttribute
    {
        private static IrcCommandList<T> commandList =
            new IrcCommandList<T>();

        public override void Process(IrcCommand command)
        {
            base.Process(command);

            var format = command.Name == "help" ?
                "I'll respond to these commands: {2}" :
                "I'll respond to these {1} commands: {2}";

            SendChannelMessage(
                string.Format(
                    format,
                    command.Bot.Configuration.NickName,
                    command.Name,
                    commandList.ToString()
                )
            );
        }
    }
}
