using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Commands
{
    public interface IIrcCommandProcessor
    {
        void Process(IrcCommand command);
    }
}
