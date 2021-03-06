﻿using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.ElasticLoadBalancing.Model;


namespace Bot.Infrastructure.AWS
{
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

            var instanceList = elbStatus.Values.ToList();
            foreach (var instance in instanceList)
            {
                if (instance.MarkedForRemoval && !states.Any(s => s.InstanceId == instance.State.InstanceId))
                {
                    elbStatus.TryRemove(instance.State.InstanceId, out notUsed);
                }
                else if (!states.Any(s => s.InstanceId == instance.State.InstanceId))
                {
                    instance.MarkedForRemoval = true;
                }
            }
        }

        public static void Clear()
        {
            instances.Clear();
        }

        public static void Clear(string elbName)
        {
            ConcurrentDictionary<string, OutTimeState> notUsed;
            instances.TryRemove(elbName, out notUsed);
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
