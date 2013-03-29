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
        private string elbName;

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
           
            return GetStatusCountMessage(states);
        }

        private string GetStatusCountMessage(List<InstanceState> instanceStates)
        {
            var stateMap = instanceStates
                .GroupBy(s => s.State)
                .ToDictionary(
                    group => group.Key,
                    group => group.Count()
                );

            int currentIn, currentOut;
            stateMap.TryGetValue("InService", out currentIn);
            stateMap.TryGetValue("OutOfService", out currentOut);

            var header = string.Format(
                    @"ELB {0} // in: {1} out: {2}",
                    this.elbName,
                    currentIn,
                    currentOut
            );

            var states = string.Join(", ",
                ElbState.GetStates(this.elbName)
                .Select(state =>
                    string.Format(
                        "{0} ({1})",
                        state.State.InstanceId,
                        state.TimeSincePulled()
                    )
                )
                .ToArray()
            );

            var message = string.Format(
                "{0} {1} {2}",
                header,
                states.Length > 0 ? "//" : string.Empty,
                states
            );

            return message;
        }

        private List<InstanceState> GetInstanceStates()
        {
            this.elbName = command.Parameters.First();
            var states = elb.InstanceState(elbName);
            if(states != null && states.Count>0)
                ElbState.UpdateStatus(elbName, states);
            return states;
        }

    }
}
