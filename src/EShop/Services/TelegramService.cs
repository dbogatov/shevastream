using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NetTelegramBotApi;
using NetTelegramBotApi.Requests;
using NetTelegramBotApi.Types;

namespace EShop.Services
{
    public class TelegramSender : ITelegramSender
    {
        public Task SendMessageAsync(string message)
        {
            return Task.Run(() => RunBot(message));
        }
		
		/// https://github.com/justdmitry/NetTelegramBotApi/blob/master/TelegramBotDemo-vNext/Program.cs
        private void RunBot(string message)
        {
			var accessToken = "178676300:AAFsnodA9QBcLF_rRgHdWYaCmwXT7JAydao";
			var chatId = 531539;

			new TelegramBot(accessToken).MakeRequestAsync(new SendMessage(chatId, message)).Wait();
        }
    }
}
