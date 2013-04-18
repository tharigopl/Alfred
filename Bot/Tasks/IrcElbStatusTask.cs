using Amazon.ElasticLoadBalancing.Model;
using Bot.Formatters;
using Bot.Infrastructure;
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
        private string statesFormatted;
        private EC2 ec2;

        public IrcElbStatusTask(string elbName)
        {
            this.elbName = elbName;
            this.Name = "Elb Status Task";
            
            this.elb = new ELB();
            this.ec2 = new EC2();

            this.SleepTime = TimeSpan.FromSeconds(15);
        }

        protected override void Execute()
        {
            GetState();
            UpdateState();
            if (StateChanged())
            {
                SendMessage(FormatMessage());
                SaveState();
            }
            //UpdateAlerts();
            RebootExpiredInstances();
        }

        protected override void OnPaused()
        {
            ElbState.Clear();
        }

        private void UpdateAlerts()
        {
            IInstanceAlert alert = new S3InstanceAlert();
            alert.TryUpdateAlert(this.currentOut);
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

            this.statesFormatted = string.Join(", ",
                ElbState.GetStates(this.elbName)
                .Select(state => state.Format())
                .ToArray()
            );
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

            var message = string.Format(
                "{0} {1} {2}",
                header,
                this.statesFormatted.Length > 0 ? "//" : string.Empty,
                this.statesFormatted
            );

            return message;
        }

        private void RebootExpiredInstances()
        {
            var tenMinutesAgo = SystemTime.Now().AddMinutes(-10);

            var expiredInstances = ElbState.GetStates(this.elbName)
                                   .Where(s => 
                                       tenMinutesAgo > s.TimeRemoved && 
                                       s.ReadyForReboot(tenMinutesAgo))
                                   .Select(s => s)
                                   .ToList();

            var expiredInstanceIds = expiredInstances.Select(s => s.State.InstanceId).ToArray();

            if (expiredInstances.Count <= 0) return;

            this.ec2.RebootInstances(expiredInstanceIds);

            var message = string.Format(
                "{0} instance{1} out > 10m, rebooting: {2}",
                expiredInstances.Count,
                expiredInstances.Count > 1 ? "s" : string.Empty,
                string.Join(", ", expiredInstances.Select(i => i.Format()))
            );

            foreach (var instance in expiredInstances)
                instance.Rebooted();

            SendMessage(message);
        }
        
    }

}
