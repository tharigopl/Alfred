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
        private SyndicationItem build; 

        public IEnumerable<string> Format(SyndicationItem build)
        {
            this.build = build;

            var heading = GetHeaderText();
            var tests = GetTestMessage();
            var changes = GetChangesUrl();

            return new List<string> {
                heading, 
                tests,
                changes
            };
        }

        private string GetHeaderText()
        {
            var title = build.Title.Text;
            var publishDate = build.PublishDate.ToLocalTime().ToString("T");

            return string.Format("{0} @ {1}", title, publishDate);
        }

        private string GetTestMessage()
        {

            var summary = build.Summary.Text;
            var startTag = "<strong>";
            var startPosition = summary.IndexOf(startTag) + startTag.Length;

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

            return testSummary;
        }

        private string GetChangesUrl()
        {
            var link = build.Links.First();
            if (link != null)
            {
                var baseUrl = link.Uri.ToString();
                var url = baseUrl + "&tab=buildChangesDiv";
                return string.Format(
                    "changes: {0}",
                    url
                );
            }

            return string.Empty;
        }
    }
}
