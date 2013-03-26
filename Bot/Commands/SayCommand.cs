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
            base.Process(command);

            if (!HasAdminUser())
                return;

            if (command.Parameters.Length < 2)
            {
                SendMessage(
                    "You have to tell me what to say and where to say it: say <channel | user> message"
                );
                return;
            }

            var target = command.Parameters[0];
            var message = string.Join(" ", command.Parameters, 1, command.Parameters.Length - 1);
            command.Client.LocalUser.SendMessage(
                target,
                message
            );
        }
    }
}
