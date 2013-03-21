using System;
using Bot.Commands.Attributes;

namespace Bot.Commands
{
	[IrcCommand("roll-dice")]
	public class DiceCommand : IrcCommandProcessor
	{
		public override void Process(IrcCommand command)
		{
			base.Process(command);

			const int dice = 2;
			const int sides = 6;

			var random = new Random();
			var roll = random.Next(1, dice * sides);
			var result = "I rolled a " + roll;
			
			SendChannelMessage(result);
		}
	}
}