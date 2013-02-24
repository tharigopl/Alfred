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

        private void ParseParameters(string[] parameters)
        {
            if (parameters.Length > 0)
                this.Name = parameters[1].ToLower();

            var commandParameters = TrimParameters(parameters);
            this.Parameters = commandParameters;
        }

        private string[] TrimParameters(string[] parameters)
        {
            var parametersToTrim = 2;

            if (parameters.Length > parametersToTrim)
            {
                var newLength = parameters.Length - parametersToTrim;
                var commandParameters = new string[parameters.Length - 2];
                Array.Copy(parameters, 2, commandParameters, 0, parameters.Length - 2);
                return commandParameters;
            }

            return new string[0];
        }

    }
}
