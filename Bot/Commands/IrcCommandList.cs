using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands.Attributes;
using Bot.Extensions;

namespace Bot.Commands
{
    public class IrcCommandList<T> where T: IrcCommandAttribute
    {
        private readonly Dictionary<string, Type> commandMap =
            typeof(IrcCommandProcessor).SubclassesWithAttribute<T>();

        public IEnumerable<string> All()
        {
            return this.commandMap
                .Select(p => p.Key)
                .OrderBy(p => p);
        }

        public override string ToString()
        {
            return AsMessage();
        }

        private string AsMessage()
        {
            return string.Join(
                ", ", 
                All().ToArray()
            );
        }
    }
}
