using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Tasks
{
    public class IrcTask : IIrcTask
    {
        public string Name { get; set; }
        public Task Task { get; private set; }
        public event EventHandler<IrcTaskMessageEventArgs> OnMessage;

        protected Action action;
        protected CancellationToken cancellationToken;

        private CancellationTokenSource tokenSource;

        public IrcTask()
        {
            this.tokenSource = new CancellationTokenSource();
            this.cancellationToken = tokenSource.Token;
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

        public void Stop()
        {
            this.tokenSource.Cancel();
        }

        public void Start()
        {
            if (action != null)
            {
                Task = Task.Run(() => TryAction(), cancellationToken);
            }
        }

        private void TryAction()
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                // output this somewhere
                // HACK: console for now
                Console.WriteLine(
                    string.Format(
                        "Exception in {0}: {1}",
                        this.Name,
                        ex.Message
                    )
                );
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
