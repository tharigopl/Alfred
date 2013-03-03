using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands.Attributes;
using Bot.Extensions;

namespace Bot.Commands
{
    [IrcCommand("help")]
    public class HelpCommand : IrcHelpCommandProcessor<IrcCommandAttribute> { }
}
