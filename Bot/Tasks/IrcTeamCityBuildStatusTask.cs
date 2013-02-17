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
        private Uri feedUri;
        private readonly IIrcMessageFormatter<SyndicationItem> formatter;

        public IrcTeamCityBuildStatusTask(Uri feedUri)
        {
            this.formatter = new BuildStatusFormatter();

            this.feedUri = feedUri;
            this.Name = "TeamCityBuild";
            this.action = () => this.Run();
        }

        private int lastBuildNumberReported = 0;

        public void Run()
        {
            while (!this.cancellationToken.IsCancellationRequested)
            {
                var build = GetMostRecentBuild();

                if (IsNewBuild(build))
                {
                    var messages = this.formatter.Format(build);
                    SendMessages(messages);
                }

                Thread.Sleep(5000);
            }
        }

        private bool IsNewBuild(SyndicationItem build)
        {
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
            var messages = new List<string>();
            //var path = @"C:\Users\Administrator\Desktop\tcfeed.xml";
            var reader = XmlReader.Create(this.feedUri.ToString());
            var feed = SyndicationFeed.Load(reader);

            return feed.Items.First();
        }

    }
}
