using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands.Attributes;
using Bot.Extensions;

namespace Bot.Commands.Tasks
{
    [IrcCommand("tasks")]
    public class TasksCommand : IrcCommandProcessor
    {
        private static IrcCommandProcessorFactory processorFactory = 
            new IrcCommandProcessorFactory(
                typeof(IrcCommandProcessor).SubclassesWithAttribute<TasksCommandAttribute>()
            );

        public override void Process(IrcCommand command)
        {
            base.Process(command);

            if (!HasParameters)
            {
                PrintTaskList();
            }
            else
            {
                command.Shift();
                var processor = processorFactory.CreateByCommand(command);
                processor.Process(command);
            }

        }

        private void PrintTaskList()
        {
            var tasks = command.Bot.TaskList;

            for (var i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                var taskStatus =
                    task.IsPaused ? 
                    "Paused" : task.IsRunning ? 
                    "Running" : "Stopped";

                SendMessage(
                    string.Format(
                        "{0}: {1} ({2})",
                        i + 1,
                        task.Name,
                        taskStatus
                    )
                );
            }

        }

    }
}
