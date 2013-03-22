using System;

namespace Bot.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AwsElbRebootCommandAttribute : IrcCommandAttribute
    {
        public AwsElbRebootCommandAttribute(string name) : base(name) { }
    }
}
