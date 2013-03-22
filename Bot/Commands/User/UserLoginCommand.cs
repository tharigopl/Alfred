using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands.Attributes;
using Bot.Data;

namespace Bot.Commands.User
{
    [UserCommand("login")]
    public class UserLoginCommand : IrcCommandProcessor
    {
        public override void Process(IrcCommand command)
        {
            base.Process(command);
            var helpCommand = new UserLoginHelpCommand();

            if (HandleNoParameters(string.Empty, helpCommand, false))
                return;

            if (command.Parameters[0] == "help" || !ValidParameters())
            {
                helpCommand.Process(command);
                return;
            }

            var username = command.Parameters[0];
            var password = command.Parameters[1];
            string message = string.Empty;
            Data.User user;

            if (!TryLogin(username, password, out message, out user))
            {
                SendPrivateMessage(message);
            }

            command.Bot.RegisterUser(
                new IrcBotUser {
                    NickName = command.Source.Name
                }
            );
        }

        private bool TryLogin(string username, string password, out string message, out Data.User user)
        {
            var repo = new UsersRepository();
            Users users;

            if (repo.TryGet(out users))
            {
                user = users
                    .Items
                    .FirstOrDefault(u =>
                        u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase)
                    );

                if (user == null)
                {
                    message = "That user is not registered.";
                    return false;
                }

                if (user.Password != password)
                {
                    message = "Incorrect password.";
                    return false;
                }

                message = "Login successful.";
                return true;
            }

            throw new DataException("Unable to retrieve users from database.");
        }

        private bool ValidParameters()
        {
            if (command.Parameters.Length < 2)
                return false;

            return true;
        }
    }

}
