using System;
using System.Threading.Tasks;

namespace Bot.Tasks
{
    public interface IIrcTask
    {
        string Name { get; set; }
        Task Task { get; }
        void Start();
        void Stop();
        void Pause();
        void Resume();
        bool IsRunning { get; }
        bool IsPaused { get; }
        event EventHandler<IrcTaskMessageEventArgs> OnMessage;
    }
}
