using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bot.Commands;
using Bot.Commands.Attributes;

namespace Bot.Extensions
{
    public static class TypeExtensions
    {
        public static Dictionary<string, Type> SubclassesWithAttribute<T>(this Type type) where T: IrcCommandAttribute
        {
            var typeMap =
                Assembly.GetExecutingAssembly().GetTypes()
                .Where(t =>
                       t.IsSubclassOf(type) &&
                       t.GetCustomAttributes<T>(false)
                        .Where(a => !a.GetType().IsSubclassOf(typeof(T)))
                        .Any()
                )
                .Select(t =>
                        new Tuple<Type, T>(
                            t,
                            t.GetCustomAttributes<T>(false).First()
                        )
                )
                .ToDictionary(t => t.Item2.Name, t => t.Item1);

            return typeMap;
        }
    }
}
