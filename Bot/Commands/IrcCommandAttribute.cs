using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IrcCommandAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public IrcCommandAttribute(string name)
        {
            this.Name = name;
        }
    }
}
