using System.Net;
using System.Threading.Tasks;
using Bot.Commands.Attributes;

namespace Bot.Commands
{
	[IrcCommand("snack")]
	public class SnackCommand : IrcCommandProcessor
	{
		private static readonly string[] Responses = new string[]
		{
			"Om nom nom!",
			"That's very nice of you!",
			"Oh thx, have a cookie yourself!",
			"Thank you very much.",
			"Thanks for the treat!"
		};

		public override void Process(IrcCommand command)
		{
			base.Process(command);
			SendMessage(Responses.Random());
		}
	}
}