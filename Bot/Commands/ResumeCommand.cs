using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands.Attributes;

namespace Bot.Commands
{
    [IrcCommand("resume")]
    public class ResumeCommand : IrcCommandProcessor
    {
        public override void Process(IrcCommand command)
        {
            base.Process(command);

            command.Bot.Resume();
            SendMessage(
                string.Format(
                    "What are the legal ramifications of bringing me back from the dead, {0}?",
                    command.Source.Name
                )
            );
        }
    }
}
