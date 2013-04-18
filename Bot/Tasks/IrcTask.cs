using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Tasks
{
    public abstract class IrcTask : IIrcTask
    {
        public string Name { get; set; }
        public Task Task { get; private set; }


        /// <summary>
        /// The time to wait between task executions. Defaults to 30s.
        /// </summary>
        public TimeSpan SleepTime { get; set; }
        public event EventHandler<IrcTaskMessageEventArgs> OnMessage;

        protected CancellationToken cancellationToken;
        private CancellationTokenSource tokenSource;

        protected IrcTask()
        {
            this.tokenSource = new CancellationTokenSource();
            this.cancellationToken = tokenSource.Token;
            this.SleepTime = TimeSpan.FromSeconds(30);
        }

        protected IrcTask(string name) : this()
        {
            this.Name = name ?? "IrcTask";
        }

        public bool IsRunning
        {
            get
            {
                return (
                    this.Task != null && 
                    !this.Task.IsCanceled && 
                    !this.Task.IsCompleted && 
                    !this.Task.IsFaulted
                );
            }
        }

        public bool IsPaused { get; private set; }

        public void Stop()
        {
            this.tokenSource.Cancel();
        }

        public void Start()
        {
            Task = Task.Run(() => TryAction(), cancellationToken);
        }

        public void Pause()
        {
            this.IsPaused = true;
            OnPaused();
        }

        protected virtual void OnPaused() { }

        public void Resume()
        {
            this.IsPaused = false;
        }

        protected abstract void Execute();

        private void TryAction()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Execute();
                }
                catch (Exception ex)
                {
                    // output this somewhere
                    // HACK: console for now
                    Console.WriteLine(
                        "Exception in {0}: {1}",
                        this.Name,
                        ex.Message
                        );
                }

                Thread.Sleep(this.SleepTime);

                while (this.IsPaused && !cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(2000);
                }
            }
        }

        protected void SendMessage(string message)
        {
            if (OnMessage != null)
            {
                OnMessage(this, new IrcTaskMessageEventArgs(new string[] { message }));
            }
        }

        protected void SendMessages(IEnumerable<string> messages)
        {
            if (OnMessage != null)
            {
                OnMessage(this, new IrcTaskMessageEventArgs(messages)); 
            }
        }

    }
}
