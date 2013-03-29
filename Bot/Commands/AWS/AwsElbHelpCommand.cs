using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands.Attributes;

namespace Bot.Commands.AWS
{
    [AwsElbCommand("help")]
    public class AwsElbHelpCommand : IrcHelpCommandProcessor<AwsElbCommandAttribute> { }
}
