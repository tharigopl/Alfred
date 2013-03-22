using IrcDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Commands
{
    public class IrcCommand
    {
        public IrcBot Bot { get; protected set; }
        public IrcBotUser User { get; protected set; }
        public IrcClient Client { get; protected set; }
        public string Name { get; protected set; }
        public string[] Parameters { get; protected set; }
        public IIrcMessageTarget Target { get; protected set; }
        public IIrcMessageSource Source { get; protected set; }

        protected IrcCommand() { }

        public IrcCommand(
            IrcBot bot, 
            IrcBotUser user,
            IrcClient client, 
            string[] parameters, 
            IIrcMessageTarget target, 
            IIrcMessageSource source
        )
        {
            this.Bot = bot;
            this.User = user;
            this.Client = client;
            this.Target = target;
            this.Source = source;

            ParseParameters(parameters);
        }

        public void Shift()
        {
            this.Name = Parameters[0].ToLower();
            TrimParameters(1);
        }

        public bool HasUser
        {
            get { return User != null; }
        }

        protected virtual void ParseParameters(string[] parameters)
        {
            if (parameters.Length > 1)
                this.Name = parameters[1].ToLower();

            this.Parameters = parameters;

            TrimParameters(2);
        }

        protected void TrimParameters(int parametersToTrim)
        {
            if (Parameters.Length >= parametersToTrim)
            {
                var newLength = Parameters.Length - parametersToTrim;
                var commandParameters = new string[Parameters.Length - parametersToTrim];
                Array.Copy(Parameters, parametersToTrim, commandParameters, 0, Parameters.Length - parametersToTrim);
                Parameters = commandParameters;
            }
        }

    }
}
