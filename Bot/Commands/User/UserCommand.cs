using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands.Attributes;
using Bot.Extensions;

namespace Bot.Commands.User
{
    [IrcCommand("user")]
    public class UserCommand : IrcCommandProcessor
    {
        private static IrcCommandProcessorFactory processorFactory = 
            new IrcCommandProcessorFactory(
                typeof(IrcCommandProcessor).SubclassesWithAttribute<UserCommandAttribute>()
            );

        public override void Process(IrcCommand command)
        {
            base.Process(command);

            var message = string.Format(
                "I need more information, {0}.",
                command.Source.Name
            );

            if (HandleNoParameters(message, new UserHelpCommand()))
                return;

            command.Shift();
            var processor = processorFactory.CreateByCommand(command);
            processor.Process(command);
        }
    }
}
