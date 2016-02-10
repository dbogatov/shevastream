using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Services
{
    public interface ITelegramSender
    {
        Task SendMessageAsync(string message);
    }
}
