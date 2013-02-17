using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bot.Commands
{
    public class IrcCommandProcessor : IIrcCommandProcessor
    {
        protected IrcCommand command;

        public virtual void Process(IrcCommand command)
        {
            this.command = command;
        }

        public void SendMessage(string message)
        {
            if (this.command == null) return;

            this.command.Client.LocalUser.SendMessage(
                this.command.Target,
                message
            );
        }
    }
}
