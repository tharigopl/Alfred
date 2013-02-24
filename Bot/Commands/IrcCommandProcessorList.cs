using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Commands
{
    public class IrcCommandProcessorList : Lazy<Dictionary<string, Type>>
    {
        public IrcCommandProcessorList() : base(GetCommandProcessors) { }

        public Dictionary<string, Type> Processors
        {
            get { return this.Value; }
        }

        private static Dictionary<string, Type> GetCommandProcessors()
        {
            var baseType = typeof(IrcCommandProcessor);

            var commandProcessors =
                Assembly.GetExecutingAssembly().GetTypes()
                .Where(t =>
                       t.IsSubclassOf(baseType) &&
                       t.GetCustomAttributes(typeof(IrcCommandAttribute), false).Length > 0)
                .Select(t =>
                        new Tuple<Type, IrcCommandAttribute>(
                            t,
                            t.GetCustomAttributes<IrcCommandAttribute>(false).First()
                        )
                )
                .ToDictionary(t => t.Item2.Name, t => t.Item1);

            return commandProcessors;
        }
    }
}
