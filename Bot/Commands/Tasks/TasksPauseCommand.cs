using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands.Attributes;
using Bot.Tasks;

namespace Bot.Commands.Tasks
{
    [TasksCommand("pause")]
    public class TasksPauseCommand : IrcCommandProcessor
    {
        public override void Process(IrcCommand command)
        {
            base.Process(command);

            if (HandleNoParameters("usage: pause <task #>"))
                return;

            int taskNumber = 0;
            if (int.TryParse(command.Parameters.FirstOrDefault(), out taskNumber))
            {
                IIrcTask task;
                if (command.Bot.TryPauseTask(taskNumber, command, out task))
                {
                    SendMessage(string.Format("{0} has been paused.", task.Name));
                }
            }
        } 
    }
}
