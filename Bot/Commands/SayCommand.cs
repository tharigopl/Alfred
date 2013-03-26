using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands.Attributes;

namespace Bot.Commands
{
    [IrcCommand("say")]
    public class SayCommand : IrcCommandProcessor
    {
        public override void Process(IrcCommand command)
        {
            if (!HasAdminUser())
                return;

            if (HandleNoParameters("You have to tell me what to say."))
                return;
        }
    }
}
