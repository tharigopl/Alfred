using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Tasks
{
    public class IrcTaskMessageEventArgs : EventArgs
    {
        public IEnumerable<string> Messages { get; set; }

        public IrcTaskMessageEventArgs(IEnumerable<string> messages)
        {
            if (messages == null)
                messages = new string[0];

            this.Messages = messages;
        }

    }
}
