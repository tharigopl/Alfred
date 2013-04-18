using Bot.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Bot.Tasks
{
    public class IrcTeamCityBuildStatusTask : IrcTask
    {
        private readonly Uri feedUri;
        private readonly IIrcMessageFormatter<SyndicationItem> formatter;
        private int lastBuildNumberReported = 0;

        public IrcTeamCityBuildStatusTask(Uri feedUri, string name = null)
            : base(name ?? "TeamCity Build Status Task")
        {
            this.formatter = new BuildStatusFormatter();

            this.feedUri = feedUri;
        }

        protected override void Execute()
        {
            var build = GetMostRecentBuild();

            if (IsNewBuild(build))
            {
                var messages = this.formatter.Format(build);
                SendMessages(messages);
            }
        }

        private bool IsNewBuild(SyndicationItem build)
        {
            if (build == null) return false;

            var buildNumber = GetBuildNumberFromBuild(build);
            var isNew = (buildNumber > lastBuildNumberReported);
            lastBuildNumberReported = buildNumber;
            return isNew;
        }

        private int GetBuildNumberFromBuild(SyndicationItem build)
        {
            var tokens = build.Title.Text.Split(' ');
            var buildNumberToken = tokens[2];
            var buildNumberText = buildNumberToken.Substring(1);
            var buildNumber = int.Parse(buildNumberText);

            return buildNumber;
        }

        private SyndicationItem GetMostRecentBuild()
        {
            SyndicationItem item = null;

            try
            {
                var reader = XmlReader.Create(this.feedUri.ToString());
                var feed = SyndicationFeed.Load(reader);

                if (feed == null) return null;

                item = feed.Items.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // HACK: console for now
                Console.WriteLine(ex);
            }

            return item;
        }

    }
}
