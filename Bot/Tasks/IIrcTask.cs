using System;
using System.Threading.Tasks;

namespace Bot.Tasks
{
    interface IIrcTask
    {
        string Name { get; set; }
        Task Task { get; }
        void Start();
        void Stop();
        bool IsRunning { get; }
        event EventHandler<IrcTaskMessageEventArgs> OnMessage;
    }
}
