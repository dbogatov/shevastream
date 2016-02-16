using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetTelegramBotApi;
using NetTelegramBotApi.Requests;
using NetTelegramBotApi.Types;

namespace EShop.Services
{
	public interface ITelegramSender
	{
		Task SendMessageAsync(string message);
		Task GetChatIdAsync();
	}

	public class TelegramSender : ITelegramSender
	{
		public Task SendMessageAsync(string message)
		{
			return Task.Run(() => RunBot(message));
		}
		
		public Task GetChatIdAsync()
		{
			var accessToken = "178676300:AAFsnodA9QBcLF_rRgHdWYaCmwXT7JAydao";

			return Task.Run(() => GetChatId(accessToken));
		}

		/// https://github.com/justdmitry/NetTelegramBotApi/blob/master/TelegramBotDemo-vNext/Program.cs
		private void RunBot(string message)
		{
			var accessToken = "178676300:AAFsnodA9QBcLF_rRgHdWYaCmwXT7JAydao";
			var chatId = -109203696; //190910699; //531539;

			new TelegramBot(accessToken).MakeRequestAsync(new SendMessage(chatId, message)).Wait();
		}
		
		private void GetChatId(string accessToken)
		{
			var bot = new TelegramBot(accessToken);

			var me = bot.MakeRequestAsync(new GetMe()).Result;
			if (me == null)
			{
				Console.WriteLine("GetMe() FAILED. Do you forget to add your AccessToken to config.json?");
				return;
			}
			Console.WriteLine("{0} (@{1}) connected!", me.FirstName, me.Username);

			Console.WriteLine();
			Console.WriteLine("Find @{0} in Telegram and send him a message - it will be displayed here", me.Username);
			Console.WriteLine();

			while (true)
			{
				var updates = bot.MakeRequestAsync(new GetUpdates() { Offset = 0 }).Result;
				if (updates != null)
				{
					foreach (var update in updates)
					{
						Console.WriteLine($"ChatID: {update.Message.Chat.Id}");
						//break;
					}
					//break;
				}
			}
		}
	}
}
