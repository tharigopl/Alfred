using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Extensions;
using Bot.Infrastructure;
using Bot.Infrastructure.AWS;

namespace Bot.Tasks
{
    public class InsightInstanceMonitor : IrcTask
    {
        private readonly EC2 ec2;
        private readonly string loadBalancerName;

        public InsightInstanceMonitor(string loadBalancerName, string name = null)
            : base(name ?? "Insight ELB Instance Monitor")
        {
            loadBalancerName.Required("loadBalancerName");

            this.loadBalancerName = loadBalancerName;
            this.ec2 = new EC2();
        }

        protected override void Execute()
        {
            RebootExpiredInstances();
        }

        private void RebootExpiredInstances()
        {
            var tenMinutesAgo = SystemTime.Now.AddMinutes(-10);

            var expiredInstances = ElbState.GetStates(this.loadBalancerName)
                                   .Where(s => 
                                       tenMinutesAgo > s.TimeRemoved && 
                                       s.ReadyForReboot(tenMinutesAgo))
                                   .Select(s => s)
                                   .ToList();

            var expiredInstanceIds = expiredInstances.Select(s => s.State.InstanceId).ToArray();

            if (expiredInstances.Count <= 0) return;

            this.ec2.RebootInstances(expiredInstanceIds);

            var message = string.Format(
                "ELB {0} // {1} instance{2} out > 10m, rebooting: {3}",
                this.loadBalancerName,
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
