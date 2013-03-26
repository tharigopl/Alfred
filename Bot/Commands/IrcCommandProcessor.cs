using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using IrcDotNet;
using Bot.Extensions;

namespace Bot.Commands
{
    public class IrcCommandProcessor : IIrcCommandProcessor
    {
        protected IrcCommand command;

        public IrcCommandProcessor() { }

        public virtual void Process(IrcCommand command)
        {
            this.command = command;
        }

        protected bool HasParameters { get { return this.command.Parameters.Length != 0; } }

        /// <summary>
        /// Override this property to provide HandleNoParameters with random phrases/messages to use.
        /// </summary>
        protected virtual IEnumerable<string> RandomMessages { get { return new string[0]; } }

        protected void SendMessage(string message, bool forcePrivate = false)
        {
            if (this.command == null) return;
            if (string.IsNullOrEmpty(message)) return;

            if (forcePrivate)
            {
                this.command.Client.LocalUser.SendMessage(
                    command.Source.Name,
                    message
                );
            }
            else
            {
                this.command.Client.LocalUser.SendMessage(
                    this.command.Target,
                    message
                );
            }
        }

        protected void SendMessages(IEnumerable<string> messages, bool forcePrivate = false)
        {
            if (this.command == null) return;

            foreach (var message in messages)
                SendMessage(message, forcePrivate);
        }

        protected void SendNotice(string message)
        {
            if (this.command == null) return;

            this.command.Client.LocalUser.SendNotice(
                this.command.Source.Name,
                message
            );
        }

        protected void SendNotices(IEnumerable<string> messages)
        {
            if (this.command == null) return;

            foreach (var message in messages)
                SendNotice(message);
        }

        protected void SendPrivateMessage(string message)
        {
            if (this.command == null) return;

            this.command.Client.LocalUser.SendMessage(
                this.command.Source.Name,
                message
            );
        }

        protected void SendPrivateMessages(IEnumerable<string> messages)
        {
            if (this.command == null) return;

            foreach (var message in messages)
                SendPrivateMessage(message);
        }

        /// <summary>
        /// If no parameters, output a random message if available then run the given command.
        /// </summary>
        /// <param name="helpCommand"></param>
        /// <returns>true if no parameters passed, false if parameters passed</returns>
        protected bool HandleNoParameters(IIrcCommandProcessor helpCommand, bool forcePrivate = false)
        {
            var prompt = this.RandomMessages.Random();
            return HandleNoParameters(prompt, helpCommand, forcePrivate);
        }

        /// <summary>
        /// If no parameters, output the given prompt.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="forcePrivate"></param>
        /// <returns>true if no parameters passed, false if parameters passed</returns>
        protected bool HandleNoParameters(string prompt, bool forcePrivate = false)
        {
            return HandleNoParameters(prompt, null, forcePrivate);
        }

        /// <summary>
        /// If no parameters, output the given prompt then run the given command.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="helpCommand"></param>
        /// <param name="forcePrivate"></param>
        /// <returns>true if no parameters passed, false if parameters passed</returns>
        protected bool HandleNoParameters(string prompt, IIrcCommandProcessor helpCommand, bool forcePrivate = false)
        {
            if (!HasParameters)
            {
                if (!string.IsNullOrEmpty(prompt))
                    SendMessage(prompt, forcePrivate);

                Thread.Sleep(1000);
                ShowHelp(helpCommand);
            }

            return !HasParameters;
        }

        protected void ShowHelp(IIrcCommandProcessor helpCommand)
        {
            if (helpCommand != null)
                helpCommand.Process(this.command);
        }

        protected bool HasAdminUser()
        {
            if (!command.HasUser || !command.User.IsAdmin)
            {
                SendMessage(
                    string.Format(
                        "I'm sorry, {0}, you need to be an administrator to use this command.",
                        command.Source.Name
                        )
                    );
                return false;
            }

            return true;
        }
    }
}
