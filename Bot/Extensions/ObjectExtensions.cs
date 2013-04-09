using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Extensions
{
    public static class ObjectExtensions
    {
        public static void Required(this object o, string paramName)
        {
            if (o == null || (o is string && string.IsNullOrEmpty(o as string)))
                throw new ArgumentException("Required parameter missing.", paramName);
        }
    }
}
