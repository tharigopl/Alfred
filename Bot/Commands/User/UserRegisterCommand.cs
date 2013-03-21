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
    [UserCommand("register")]
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

            CreateUser();
        }

        private void CreateUser()
        {
            Data.User user = null;
            if (TryCreateUser(out user))
            {
                SendRegisteredMessages(user);
            }
            else
            {
                SendErrorMessages();
            }
        }

        private bool TryCreateUser(out Data.User user)
        {
            user = ParseParameters();
            var repository = new UsersRepository();
            return repository.Save(user);
        }

        private void SendRegisteredMessages(Data.User user)
        {
            SendPrivateMessages(
                new string[] {
                    string.Format(
                        "{0}, you are now registered with the username {1} and the password {2}.",
                        command.Source.Name,
                        user.UserName,
                        user.Password
                    ),
                    string.Format(
                        "If I need to e-mail you, I will use the address you gave me: {0}",
                        user.Email
                    ),
                    "To login and receive your candy, use the command: login <username> <password>"
                }
            );
        }

        private void SendErrorMessages()
        {
            SendPrivateMessage(
                string.Format(
                    "I was unable to create a user with the username {0} because it's already registered.",
                    command.Parameters[0]
                )
            );
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
                SendPrivateMessage("username must be at least 3 characters.");
                return false;
            }

            var password = command.Parameters[1];
            if (password.Length < 5)
            {
                SendPrivateMessage("password must be at least 5 characters.");
                return false;
            }

            var email = command.Parameters[2];
            if (!EmailIsValid(email)) {
                SendPrivateMessage("invalid e-mail address.");
                return false;
            }

            return true;
        }

        static readonly Regex ValidEmailRegex = CreateValidEmailRegex();

        /// <summary>
        /// Taken from http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx
        /// </summary>
        /// <returns></returns>
        private static Regex CreateValidEmailRegex()
        {
            const string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
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
