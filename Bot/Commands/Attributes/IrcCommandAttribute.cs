using System;

namespace Bot.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IrcCommandAttribute : Attribute
    {
        public string Name { get; private set; }

        public IrcCommandAttribute(string name)
        {
            this.Name = name;
        }
    }
}
