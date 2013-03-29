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
        private readonly ELB elb;
        private int lastIn, currentIn;
        private int lastOut, currentOut;
        private List<InstanceState> states;

        public IrcElbStatusTask(string elbName)
        {
            this.elbName = elbName;
            this.Name = "Elb Status Task";
            this.action = this.Run;
            
            this.elb = new ELB();
        }

        public void Run()
        {
            while (!this.cancellationToken.IsCancellationRequested)
            {
                GetState();
                UpdateState();
                if (StateChanged())
                {
                    SendMessage(FormatMessage());
                    SaveState();
                }

                Thread.Sleep(15000);
            }
        }

        private void GetState()
        {
            this.states = elb.InstanceState(this.elbName);
        }

        private void UpdateState()
        {
            ElbState.UpdateStatus(this.elbName, states);

            this.currentIn = this.lastIn;
            this.currentOut = this.lastOut;

            var stateMap = this.states
                .GroupBy(s => s.State)
                .ToDictionary(
                    group => group.Key,
                    group => group.Count()
                );

            stateMap.TryGetValue("InService", out currentIn);
            stateMap.TryGetValue("OutOfService", out currentOut);
        }

        private bool StateChanged()
        {
            return !(lastIn == currentIn && lastOut == currentOut);
        }

        private void SaveState()
        {
            lastIn = currentIn;
            lastOut = currentOut;
        }

        private string FormatMessage()
        {
            var header = string.Format(
                    @"ELB {0} // in: {1} out: {2}",
                    this.elbName,
                    this.currentIn,
                    this.currentOut
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
        
    }
}
