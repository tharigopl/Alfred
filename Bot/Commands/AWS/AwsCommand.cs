using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bot.Commands.Attributes;
using Bot.Extensions;

namespace Bot.Commands.AWS
{
    [IrcCommand("aws")]
    public class AwsCommand : IrcCommandProcessor
    {
        private static IrcCommandProcessorFactory processorFactory = 
            new IrcCommandProcessorFactory(
                typeof(IrcCommandProcessor).SubclassesWithAttribute<AwsCommandAttribute>()
            );

        public override void Process(IrcCommand command)
        {
            base.Process(command);

            if (HandleNoParameters(new AwsHelpCommand()))
                return;

            command.Shift();
            var processor = processorFactory.CreateByCommand(command);
            processor.Process(command);
        }
    }
}
