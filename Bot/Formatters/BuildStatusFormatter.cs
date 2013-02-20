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
            var publishDate = build.PublishDate.ToLocalTime().ToString("T");

            var startPosition = summary.IndexOf("<strong>") + "<strong>".Length;
            var testSummary = summary.Substring(
                    startPosition,
                    summary.IndexOf("</strong>") - startPosition
            );

            var numberOfTestsText = testSummary.Split(' ')[2];
            var numberOfTests = int.Parse(numberOfTestsText.Substring(0, numberOfTestsText.Length - 1));
            var testsLeftToParty = 1337 - numberOfTests;
            testSummary += string.Format(
                ", {0} left to partay!", 
                testsLeftToParty
            );

            messages.Add(string.Format("{0} @ {1}", title, publishDate));
            messages.Add(testSummary);

            return messages;
        }
    }
}
