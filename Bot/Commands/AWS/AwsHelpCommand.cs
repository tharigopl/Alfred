using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands.Attributes;
using Bot.Extensions;

namespace Bot.Commands.AWS
{
    [AwsCommand("help")]
    public class AwsHelpCommand : IrcHelpCommandProcessor<AwsCommandAttribute> { }
}
