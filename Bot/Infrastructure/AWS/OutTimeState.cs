using System;
using System.Text;
using Amazon.ElasticLoadBalancing.Model;

namespace Bot.Infrastructure.AWS
{
    public class OutTimeState
    {
        public InstanceState State { get; internal set; }
        public DateTime TimeRemoved { get; internal set; }
        public DateTime LastRebooted { get; private set; }
        public int NumberOfReboots { get; private set; }
        public bool HasBeenRebooted { get; private set;  }
        public bool MarkedForRemoval { get; set; }

        public OutTimeState(InstanceState state)
        {
            HasBeenRebooted = false;
            TimeRemoved = SystemTime.Now;
            State = state;
        }

        public string TimeSincePulled()
        {
            var timeSinceRemoved = SystemTime.Now - TimeRemoved;
            return FormatTimeSpan(timeSinceRemoved);
        }

        private string FormatTimeSpan(TimeSpan ts)
        {
            if (ts.Days > 0) return ts.ToString(@"d\dh\hm\m") + " WTF!";
            if (ts.Hours > 0) return ts.ToString(@"h\hm\ms\s");
            if (ts.Minutes > 0) return ts.ToString(@"m\ms\s");

            return ts.ToString(@"s\s");
        }

        public string Format()
        {
            return string.Format(
                "{0} ({1}{2})",
                State.InstanceId,
                TimeSincePulled(),
                HasBeenRebooted ? " - rebooted" : string.Empty
            );
        }

        public void Rebooted()
        {
            HasBeenRebooted = true;
            NumberOfReboots++;
            LastRebooted = SystemTime.Now;
        }

        public bool ReadyForReboot(DateTime cutoff)
        {
            return !HasBeenRebooted || cutoff > LastRebooted;
        }

    }
}