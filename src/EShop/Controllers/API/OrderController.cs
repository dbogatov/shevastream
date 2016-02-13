using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShop.Models.Enitites;
using EShop.Services;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using EShop.ViewModels.Home;

namespace EShop.Controllers.API {

	[Produces("application/json")]
	[Route("api/Order")]
	public class OrderController : Controller {
		private readonly ITelegramSender _telegram;

		public OrderController(ITelegramSender telegram) {
			_telegram = telegram;
		}

		// POST api/order
		[HttpPost]
		public bool Post(OrderViewModel order) {

            _telegram.SendMessageAsync(order.ToString());

            return true;
		}
	}
}