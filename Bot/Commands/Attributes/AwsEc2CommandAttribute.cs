using System;

namespace Bot.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AwsEc2CommandAttribute : IrcCommandAttribute
    {
        public AwsEc2CommandAttribute(string name) : base(name) { }
    }
}
