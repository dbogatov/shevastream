using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using NetTelegramBotApi;
using NetTelegramBotApi.Requests;

namespace EShop.Services
{
	public interface ITelegramSender
	{
		Task SendMessageAsync(string message);
		Task GetChatIdAsync();
	}

	public class TelegramSender : ITelegramSender
	{
        private readonly IHostingEnvironment _env;

        public TelegramSender(IHostingEnvironment env)
		{
            _env = env;
        }
		
		public Task SendMessageAsync(string message)
		{
			if (_env.IsProduction())
			{
				return Task.Run(() => RunBot(message));	
			} else
			{
                Console.WriteLine($"TELEGRAM: {message}");
                return Task.FromResult(0);	
			}
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
			var chatId = -109203696;

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
					}
				}
			}
		}
	}
}
