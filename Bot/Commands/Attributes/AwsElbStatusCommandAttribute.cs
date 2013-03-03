using System;

namespace Bot.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AwsElbStatusCommandAttribute : IrcCommandAttribute
    {
        public AwsElbStatusCommandAttribute(string name) : base(name) { }
    }
}
