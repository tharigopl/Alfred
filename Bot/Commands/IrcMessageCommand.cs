using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrcDotNet;

namespace Bot.Commands
{
    public class IrcMessageCommand : IrcCommand
    {
        public IrcMessageCommand(
            IrcBot bot, 
            IrcBotUser user,
            IrcClient client, 
            string[] parameters, 
            IIrcMessageTarget target, 
            IIrcMessageSource source
        )
            : base (bot, user, client, parameters, target, source)
        {}

        protected override void ParseParameters(string[] parameters)
        {
            if (parameters.Length > 0)
                this.Name = parameters[0].ToLower();

            this.Parameters = parameters;

            TrimParameters(1);
        }
    }
}
