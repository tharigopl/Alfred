using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.ServiceModel.Syndication;
using Bot.Formatters;

namespace Bot.Commands
{
    public class BuildStatusCommand : IrcCommandProcessor
    {
        private readonly IIrcMessageFormatter<SyndicationItem> formatter;

        public BuildStatusCommand()
        {
            this.formatter = new BuildStatusFormatter();
        }

        public override void Process(IrcCommand command)
        {
            base.Process(command);

            var statusMessages = GetBuildStatus();
            foreach (var message in statusMessages)
                SendMessage(message);
        }

        private IEnumerable<string> GetBuildStatus()
        {
            var path = @"C:\Users\Administrator\Desktop\tcfeed.xml";
            var reader = XmlReader.Create(path);
            var feed = SyndicationFeed.Load(reader);
            var build = feed.Items.First();

            if (build != null)
            {
                return this.formatter.Format(build);
            }

            return new string[0];
        }


    }
}
