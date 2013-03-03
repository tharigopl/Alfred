using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.EC2.Model;
using Amazon.ElasticLoadBalancing.Model;
using Bot.Commands.Attributes;
using Bot.Infrastructure.AWS;

namespace Bot.Commands.AWS
{
    [AwsElbCommand("status")]
    public class AwsElbStatusCommand : IrcCommandProcessor
    {
        private readonly ELB elb;
        private readonly EC2 ec2;

        public AwsElbStatusCommand()
        {
            this.elb = new ELB();
            this.ec2 = new EC2();
        }

        public override void Process(IrcCommand command)
        {
            base.Process(command);

            var message = string.Format("I'm going to need a little more information, {0}.", command.Source.Name);
            if (HandleNoParameters(message, new AwsElbStatusHelpCommand()))
                return;

            SendMessage("checking status...");
            SendMessage(GetInstanceStatusMessage());
        }

        private string GetInstanceStatusMessage()
        {
            var instanceIds = GetInstanceIds();

            if (instanceIds == null || instanceIds.Count == 0)
                return string.Format(
                    "Sorry, {0}, but that load balancer either doesn't exist or doesn't have any instances attached to it.", 
                    this.command.Source.Name
                );

            var instanceStatus = ec2.InstanceStatus(instanceIds);
            var statusCountMessage = GetStatusCountMessage(instanceStatus);

            return string.Format(
                "{0} instances attached: {1}",
                instanceIds.Count,
                statusCountMessage
            );
        }

        private string GetStatusCountMessage(List<InstanceStatus> instanceStatus)
        {
            var statusCounts = instanceStatus
                .GroupBy(s => s.InstanceState.Name)
                .Select(g => string.Format(
                        "{0} are {1}",
                        g.Count(),
                        g.Key
                    )
                )
                .ToArray();

            var statusCountMessage =
                string.Join(
                    ", ",
                    statusCounts
                );

            return statusCountMessage;
        }

        private List<string> GetInstanceIds()
        {
            var instances = elb.Instances(command.Parameters.First());
            return instances.Select(i => i.InstanceId).ToList();
        }

    }
}
