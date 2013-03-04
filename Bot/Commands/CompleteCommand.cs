using System;
using System.Linq;
using System.Xml.Linq;
using Bot.Commands.Attributes;

namespace Bot.Commands
{
	/// <summary>
	/// Ex: alfred complete mary had a
	/// </summary>
	[IrcCommand("complete")]
	public class CompleteCommand : IrcCommandProcessor
	{
		public override void Process(IrcCommand command)
		{
			base.Process(command);

			const string url = "http://google.com/complete/search?q={0}&output=toolbar";
			var uri = new Uri(string.Format(url, string.Join(" ", command.Parameters)));

			var task = uri.Download();
			if (task.IsFaulted || task.IsCanceled)
			{
				SendMessage("Sorry master, I could not complete the phrase");
			}
			else
			{
				var xml = XDocument.Parse(task.Result);
				var suggestions = xml.Descendants("suggestion");
				if (!suggestions.Any())
				{
					SendMessage("Sorry master, I have no suggestions.");
					return;
				}

				foreach (var node in suggestions)
				{
					SendMessage(node.Attribute("data").Value);
				}
			}
		}
	}
}