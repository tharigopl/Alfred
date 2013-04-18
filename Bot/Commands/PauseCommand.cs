using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands.Attributes;

namespace Bot.Commands
{
    [IrcCommand("pause")]
    public class PauseCommand : IrcCommandProcessor
    {
        public override void Process(IrcCommand command)
        {
            base.Process(command);

            command.Bot.Pause();
            SendMessage(
                string.Format(
                    "Without the great sacrifices you've made, {0}, we wouldn't be here to share this nice pot of tea.",
                    command.Source.Name
                )
            );
        }
    }
}
