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
    [AwsElbCommand("repair")]
    public class AwsElbRepairCommand : IrcCommandProcessor
    {
        private readonly ELB elb;
        private readonly EC2 ec2;

        private List<InstanceState> states; 

        public AwsElbRepairCommand()
        {
            this.elb = new ELB();
            this.ec2 = new EC2();
        }

        public override void Process(IrcCommand command)
        {
            base.Process(command);

            var message = string.Format("I'm going to need a little more information, {0}.", command.Source.Name);
            if (HandleNoParameters(message, new AwsElbRepairHelpCommand()))
                return;

            SendMessage("checking status...");
            this.states = GetInstanceStates();
            SendMessage(GetInstanceStatusMessage());

            var badInstances = GetBadInstances();
            if (badInstances.Count > 0)
            {
                RebootInstances(badInstances);
            }
            else
            {
                SendMessage(
                    string.Format(
                        "no out of service instances detected, {0}. celebrate!",
                        this.command.Source.Name
                    )
                );
            }

        }

        private void RebootInstances(List<string> instances)
        {
            var instancesMessage = string.Join(", ", instances.Select(i => i).ToArray());
            SendMessage(
                string.Format(
                    "out of service instances detected. rebooting these now: {0}",
                    instancesMessage
                )
            );

            this.ec2.RebootInstances(instances);

            SendMessage(
                string.Format(
                    "finished rebooting: {0}",
                    instancesMessage
                )
            );
        }

        private string GetInstanceStatusMessage()
        {
            if (states == null || states.Count == 0)
                return string.Format(
                    "Sorry, {0}, but that load balancer either doesn't exist or doesn't have any instances attached to it.", 
                    this.command.Source.Name
                );

            var statusCountMessage = GetStatusCountMessage(states);

            return string.Format(
                "{0} instances attached: {1}",
                states.Count,
                statusCountMessage
            );
        }

        private List<string> GetBadInstances()
        {
            return (from state in this.states where state.State != "InService" select state.InstanceId).ToList();
        }

        private string GetStatusCountMessage(List<InstanceState> instanceStates)
        {
            var statusCounts = instanceStates
                .GroupBy(s => s.State)
                .Select(g => string.Format(
                        "{0} {1} {2}",
                        g.Count(),
                        g.Count() == 1 ? "is" : "are",
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

        private List<InstanceState> GetInstanceStates()
        {
            var elbName = command.Parameters.First();
            var states = elb.InstanceState(elbName);
            return states;
        }

    }
}
