using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bot.Commands.Attributes;
using Bot.Data;

namespace Bot.Commands.User
{
    [IrcCommand("register")]
    public class UserRegisterCommand : IrcCommandProcessor
    {
        public override void Process(IrcCommand command)
        {
            base.Process(command);
            var helpCommand = new UserRegisterHelpCommand();

            if (HandleNoParameters(string.Empty, helpCommand, false))
                return;

            if (command.Parameters[0] == "help" || !ValidParameters())
            {
                helpCommand.Process(command);
                return;
            }

            var user = CreateUser();
            SendNotices(user);
        }

        private Data.User CreateUser()
        {
            var user = ParseParameters();
            var repository = new UsersRepository();
            repository.Save(user);
            return user;
        }

        private void SendNotices(Data.User user)
        {
            SendPrivateMessage(
                string.Format(
                    "{0}, you are now registered with the username {1} and the password {2}.",
                    command.Source.Name,
                    user.UserName,
                    user.Password
                )
            );
            SendNotice(
                string.Format(
                    "If I need to e-mail you, I will use the address you gave me: {0}",
                    user.Email
                )
            );
            SendNotice("To login and receive your candy, use the command: login <username> <password>");
        }

        private Data.User ParseParameters()
        {
            var user = new Data.User
            {
                UserName = command.Parameters[0],
                Password = command.Parameters[1],
                Email = command.Parameters[2]
            };

            return user;
        }

        private bool ValidParameters()
        {
            if (command.Parameters.Length < 3 || command.Parameters.Length > 3) return false;

            var username = command.Parameters[0];
            if (username.Length < 3)
            {
                SendNotice("username must be at least 3 characters.");
                return false;
            }

            var password = command.Parameters[1];
            if (password.Length < 5)
            {
                SendNotice("password must be at least 5 characters.");
                return false;
            }

            var email = command.Parameters[2];
            if (!EmailIsValid(email)) {
                SendNotice("invalid e-mail address.");
                return false;
            }

            return true;
        }

        static Regex ValidEmailRegex = CreateValidEmailRegex();

        /// <summary>
        /// Taken from http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx
        /// </summary>
        /// <returns></returns>
        private static Regex CreateValidEmailRegex()
        {
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
        }

        private bool EmailIsValid(string emailAddress)
        {
            return ValidEmailRegex.IsMatch(emailAddress);
        }
    }
}
