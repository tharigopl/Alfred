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
    [AwsElbCommand("urls")]
    public class AwsElbInstanceUrlsCommand : IrcCommandProcessor
    {
        private List<RunningInstance> descriptions;
        private List<Amazon.ElasticLoadBalancing.Model.InstanceState> states;
        private Dictionary<string, string> stateMap; 
 
        public override void Process(IrcCommand command)
        {
            base.Process(command);

            var message = string.Format(
                "I need the name of the load balancer, {0}.",
                command.Source.Name
            );

            if (HandleNoParameters(message, null))
                return;

            GetInstanceStates();
            GetDescriptions();
            PrintUrls();
        }

        private void PrintUrls()
        {
            foreach (var description in this.descriptions)
            {
                SendMessage(
                    string.Format(
                        "http://{0} ({1})",
                        description.PublicDnsName,
                        this.stateMap[description.InstanceId]
                    )
                );
            }
        }

        private void GetDescriptions()
        {
            var ec2 = new EC2();
            this.descriptions = ec2.InstanceDescriptions(states.Select(i => i.InstanceId));
        }

        private void GetInstanceStates()
        {
            var loadBalancer = command.Parameters[0];
            var elb = new ELB();
            this.states = elb.InstanceState(loadBalancer);
            this.stateMap = states.ToDictionary(k => k.InstanceId, v => v.State);
        }
    }
}
