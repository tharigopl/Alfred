using System;
using System.Collections.Generic;
namespace Bot.Formatters
{
    interface IIrcMessageFormatter<T>
    {
        IEnumerable<string> Format(T item);
    }
}
