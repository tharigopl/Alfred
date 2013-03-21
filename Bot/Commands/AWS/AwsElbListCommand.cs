using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.ElasticLoadBalancing.Model;
using Bot.Commands.Attributes;
using Bot.Infrastructure.AWS;

namespace Bot.Commands.AWS
{
    [AwsElbCommand("list")]
    public class AwsElbListCommand : IrcCommandProcessor
    {
        private readonly ELB elb;

        public AwsElbListCommand()
        {
            this.elb = new ELB();
        }

        public override void Process(IrcCommand command)
        {
            base.Process(command);

            SendChannelMessage("retrieving list of load balancers...");
            var descriptions = this.elb.List(command.Parameters.FirstOrDefault());
            SendChannelMessages(GetMessages(descriptions));
            SendChannelMessage("use \"aws elb status <elb name>\" to see more information");
        }

        private List<string> GetMessages(List<LoadBalancerDescription> descriptions)
        {
            var messages = new List<string>();

            messages.Add(
                string.Format(
                    "found {0} load balancer{1}:",
                    descriptions.Count,
                    descriptions.Count > 1 ? "s" : ""
                )
            );

            foreach (var description in descriptions)
            {
                messages.Add(
                    string.Format(
                        "{0} / {1} / {2} instances",
                        description.LoadBalancerName,
                        description.DNSName,
                        description.Instances.Count
                    )
                );
            }

            return messages;
        }
    }
}
