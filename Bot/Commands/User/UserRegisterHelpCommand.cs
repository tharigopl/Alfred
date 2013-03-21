using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Commands.User
{
    public class UserRegisterHelpCommand : IrcCommandProcessor
    {
        public override void Process(IrcCommand command)
        {
            base.Process(command);

            SendPrivateMessage("usage: register <username> <password> <email>");
        }

    }
}
