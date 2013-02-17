using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Formatters
{
    public class BuildStatusFormatter : IIrcMessageFormatter<SyndicationItem>
    {
        public IEnumerable<string> Format(SyndicationItem build)
        {
            var messages = new List<string>();

            var title = build.Title.Text;
            var summary = build.Summary.Text;
            var publishDate = build.PublishDate.ToString("T");

            var startPosition = summary.IndexOf("<strong>") + "<strong>".Length;
            var testSummary = summary.Substring(
                    startPosition,
                    summary.IndexOf("</strong>") - startPosition
            );

            messages.Add(string.Format("{0} @ {1}", title, publishDate));
            messages.Add(testSummary);

            return messages;
        }
    }
}
