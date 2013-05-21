using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Infrastructure
{
    public static class SystemTime
    {
        private static Func<DateTime> now = () => DateTime.Now;

        public static DateTime Now
        {
            get { return now();  }
        } 

        public static DateTime UtcNow
        {
            get { return now().ToUniversalTime();  }
        } 

        public static void Set(DateTime dt)
        {
            now = () => dt;
        }

        public static void MoveForward(TimeSpan ts)
        {
            var dt = now().Add(ts);
            Set(dt);
        }
    }
}
