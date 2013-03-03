using System;

namespace Bot.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AwsElbCommandAttribute : IrcCommandAttribute
    {
        public AwsElbCommandAttribute(string name) : base(name) { }
    }
}
