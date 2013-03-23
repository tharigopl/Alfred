using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Extensions
{
    public static class IEnumerableExtensions
    {
        public static T Random<T> (this IEnumerable<T> items) {
            var count = items.Count();
            if (count == 0) return default(T);

            var r = new Random(DateTime.Now.Millisecond);
            var index = r.Next(1, count);

            return items.ElementAt(index);
        }
    }
}
