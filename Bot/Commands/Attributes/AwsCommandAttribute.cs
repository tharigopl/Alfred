using System;

namespace Bot.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AwsCommandAttribute : IrcCommandAttribute
    {
        public AwsCommandAttribute(string name) : base(name) { }
    }
}
