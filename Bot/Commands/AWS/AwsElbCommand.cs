using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands.Attributes;
using Bot.Extensions;

namespace Bot.Commands.AWS
{
    [AwsCommand("elb")]
    public class AwsElbCommand : IrcCommandProcessor
    {
        private static IrcCommandProcessorFactory processorFactory = 
            new IrcCommandProcessorFactory(
                typeof(IrcCommandProcessor).SubclassesWithAttribute<AwsElbCommandAttribute>()
            );

        public override void Process(IrcCommand command)
        {
            base.Process(command);

            if (HandleNoParameters("I really like ELB, too!", new AwsElbHelpCommand()))
                return;

            command.Shift();
            var processor = processorFactory.CreateByCommand(command);
            processor.Process(command);
        }
    }
}
