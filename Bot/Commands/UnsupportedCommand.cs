using IrcDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Commands
{
    public class UnsupportedCommand : IIrcCommandProcessor
    {
        private readonly List<string> messages = new List<string> {
            "Know your limits, Master {0}",
            "Batman has no limits, {0}, but I do.",
            "Will you be wanting the Batpod, too, {0}?",
            "Apply your own bloody suntan lotion, {0}."
        }; 

        public void Process(IrcCommand command)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var index = rand.Next(0, messages.Count);
            var message = messages[index];

            command.Client.LocalUser.SendMessage(
                command.Target, 
                string.Format(
                    message,
                    command.Source.Name
                )
            );
        }
    }
}
