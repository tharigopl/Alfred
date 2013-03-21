using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bot.Commands.Attributes;

namespace Bot.Commands.AWS
{
    [AwsElbRepairCommand("help")]
    public class AwsElbRepairHelpCommand : IrcCommandProcessor
    {
        public override void Process(IrcCommand command)
        {
            base.Process(command);

            SendChannelMessage(
                "It's easy! Just type this: aws elb repair <load balancer name>"
            );
        }
    }
}
