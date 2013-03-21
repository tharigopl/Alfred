using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands.Attributes;

namespace Bot.Commands.User
{
    [UserCommand("login")]
    public class UserLoginCommand : IrcCommandProcessor
    {
        public override void Process(IrcCommand command)
        {
            base.Process(command);
        }
    }
}
