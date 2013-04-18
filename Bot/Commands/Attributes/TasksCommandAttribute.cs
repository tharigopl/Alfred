using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TasksCommandAttribute : IrcCommandAttribute
    {
        public TasksCommandAttribute(string name): base(name) { }
    }
}
