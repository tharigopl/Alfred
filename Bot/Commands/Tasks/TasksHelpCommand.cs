using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands.Attributes;

namespace Bot.Commands.Tasks
{
    [TasksCommand("help")]
    public class TasksHelpCommand : IrcHelpCommandProcessor<TasksCommandAttribute> { }
}
