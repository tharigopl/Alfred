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
        public IrcBot Bot { get; private set; }
        public IrcClient Client { get; private set; }
        public string Name { get; private set; }
        public string[] Parameters { get; private set; }
        public IIrcMessageTarget Target { get; private set; }
        public IIrcMessageSource Source { get; private set; }

        public IrcCommand(
            IrcBot bot, 
            IrcClient client, 
            string[] parameters, 
            IIrcMessageTarget target, 
            IIrcMessageSource source
        )
        {
            this.Bot = bot;
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

        private void ParseParameters(string[] parameters)
        {
            if (parameters.Length > 1)
                this.Name = parameters[1].ToLower();

            this.Parameters = parameters;
            TrimParameters(2);
        }

        private void TrimParameters(int parametersToTrim)
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
