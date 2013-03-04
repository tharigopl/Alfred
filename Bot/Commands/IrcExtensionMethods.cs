using System;
using System.Net;
using System.Threading.Tasks;

namespace Bot.Commands
{
	public static class IrcExtensionMethods
	{
		//TODO: move this class somewhere interesting _or_ delete this todo
		public static T Random<T>(this T[] list)
		{
			var random = new Random();
			return list[random.Next(list.Length)];
		}

		public async static Task<string> Download(this Uri uri)
		{
			var client = new WebClient();
			var downloadStringTask = client.DownloadStringTaskAsync(uri);
			var result = await downloadStringTask;
			return result;
		}
	}
}