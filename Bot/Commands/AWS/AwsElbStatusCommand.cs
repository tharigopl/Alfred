using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.ElasticLoadBalancing.Model;
using Bot.Commands.Attributes;
using Bot.Infrastructure.AWS;
using Bot.Data;

namespace Bot.Commands.AWS
{
    [AwsElbCommand("status")]
    public class AwsElbStatusCommand : IrcCommandProcessor
    {
        private readonly ELB elb;

        public AwsElbStatusCommand()
        {
            this.elb = new ELB();
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
            var states = GetInstanceStates();

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
            if(states != null && states.Count>0)
                ElbState.UpdateStatus(elbName, states);
            return states;
        }

    }
}
