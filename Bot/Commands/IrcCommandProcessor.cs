using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IrcDotNet;

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

        protected void SendMessage(string message)
        {
            if (this.command == null) return;

            this.command.Client.LocalUser.SendMessage(
                this.command.Target,
                message
            );
        }

        protected void SendNotice(string message)
        {
            if (this.command == null) return;

            this.command.Client.LocalUser.SendNotice(
                new string[1] { this.command.Source.Name },
                message
            );
        }


    }
}
