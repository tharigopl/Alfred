using System;
using System.Text;
using Amazon.ElasticLoadBalancing.Model;

namespace Bot.Infrastructure.AWS
{
    public class OutTimeState
    {
        public InstanceState State { get; internal set; }
        public DateTime TimeRemoved { get; internal set; }
        public bool Rebooted { get; set;  }

        public OutTimeState(InstanceState state)
        {
            Rebooted = false;
            TimeRemoved = SystemTime.Now();
            State = state;
        }

        public string TimeSincePulled()
        {
            var timeSinceRemoved = SystemTime.Now() - TimeRemoved;
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
                Rebooted ? " - rebooted" : string.Empty
            );
        }

    }
}