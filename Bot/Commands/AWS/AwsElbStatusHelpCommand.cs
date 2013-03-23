using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bot.Commands.Attributes;

namespace Bot.Commands.AWS
{
    [AwsElbStatusCommand("help")]
    public class AwsElbStatusHelpCommand : IrcCommandProcessor
    {
        public override void Process(IrcCommand command)
        {
            base.Process(command);

            SendMessage(
                "It's easy! Just type this: aws elb status <load balancer name>. If you need a list of load balancers, that command is: aws elb list."
            );
        }
    }
}
