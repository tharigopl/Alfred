using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.ElasticLoadBalancing.Model;


namespace Bot
{
    public static class TimeSpanHelper
    {
        public static string DayHourMinSecString(this TimeSpan span)
        {
            if (span.Days > 0)
            {
                return string.Format("{0} days, {1}:{2}:{3}", span.Days, span.Hours, span.Minutes, span.Seconds);
            }
            else if (span.Hours > 0)
            {
                return string.Format("{0}:{1}:{2} sec", span.Hours, span.Minutes, span.Seconds);
            }
            else if (span.Minutes > 0)
            {
                return string.Format("{0}:{1} sec", span.Minutes, span.Seconds);
            }
            return string.Format("{0} sec", span.Seconds);
        }
    }

    public class OutTimeState
    {
        public InstanceState State { get; internal set; }
        public DateTime TimeRemoved { get; internal set; }

        public OutTimeState(InstanceState state)
        {
            TimeRemoved = DateTime.Now;
            State = state;
        }

        public string TimeSincePulled()
        {
            var timeSinceRemoved = DateTime.Now - TimeRemoved;
            return timeSinceRemoved.DayHourMinSecString();
        }

    }
    
    public class ElbState
    {
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, OutTimeState>> instances = new ConcurrentDictionary<string, ConcurrentDictionary<string, OutTimeState>>();

        public static void UpdateStatus(string ElbName, List<InstanceState> states)
        {
            ConcurrentDictionary<string, OutTimeState> elbStatus;
            if (!instances.TryGetValue(ElbName, out elbStatus))
            {
                elbStatus =  new ConcurrentDictionary<string, OutTimeState>();
                instances.TryAdd(ElbName, elbStatus);
            }
            OutTimeState notUsed;
            states.ForEach(instanceState =>
            {
                if (instanceState.State == "InService")
                {
                    
                    elbStatus.TryRemove(instanceState.InstanceId, out notUsed);
                }
                else if (instanceState.State == "OutOfService" &&
                    !elbStatus.TryGetValue(instanceState.InstanceId, out notUsed))
                {
                    elbStatus.TryAdd(instanceState.InstanceId, new OutTimeState(instanceState));
                }

            });
        }

        public static ICollection<OutTimeState> GetStates(string ElbName)
        {
            ConcurrentDictionary<string, OutTimeState> elbStates;
            if (instances.TryGetValue(ElbName, out elbStates))
            {
                return elbStates.Values;
            }
            return new List<OutTimeState>();
        }
    }
}
