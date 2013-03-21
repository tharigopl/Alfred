using System;

namespace Bot.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AwsElbRepairCommandAttribute : IrcCommandAttribute
    {
        public AwsElbRepairCommandAttribute(string name) : base(name) { }
    }
}
