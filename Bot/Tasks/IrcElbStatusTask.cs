using Amazon.ElasticLoadBalancing.Model;
using Bot.Formatters;
using Bot.Infrastructure.AWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Bot.Tasks
{
    public class IrcElbStatusTask : IrcTask
    {
        private readonly string elbName;
        private readonly IIrcMessageFormatter<SyndicationItem> formatter;
        private int lastBuildNumberReported = 0;
        private readonly ELB elb;
        private readonly EC2 ec2;
        private DateTime LastCheckTime;
        private int InCount;
        private int OutCount;

        public IrcElbStatusTask(string elbName)
        {
            this.formatter = new BuildStatusFormatter();

            this.elbName = elbName;
            this.Name = "Elb Status Task";
            this.action = this.Run;
            
            this.elb = new ELB();
            this.ec2 = new EC2();
        }

        public void Run()
        {
            while (!this.cancellationToken.IsCancellationRequested)
            {
                var states = elb.InstanceState(this.elbName);

                if (StatesChanged(states))
                {
                    
                    SendMessages(FormatMessage());
                }

                Thread.Sleep(15000);
            }
        }

        private IEnumerable<string> FormatMessage()
        {
            var messages = new List<string>{
                string.Format("Elb {0}: InService: {1}, OutOfService: {2}", this.elbName, this.InCount, this.OutCount)
            };

            foreach (var state in ElbState.GetStates(this.elbName))
            {
                messages.Add(string.Format("Instance {0} has been out for {1}", state.State.InstanceId, state.TimeSincePulled()));
            }

            return messages;
        }

        private bool StatesChanged(List<InstanceState> states)
        {
            ElbState.UpdateStatus(this.elbName, states);
            bool changed = false;
            var inService = states.Count(s => s.State == "InService");
            var outOfService = states.Count(s => s.State == "OutOfService");
            if (InCount != inService || OutCount != outOfService)
            {
                changed = true;
            }
            InCount = inService;
            OutCount = outOfService;
            return changed;

        }

        
        
    }
}
