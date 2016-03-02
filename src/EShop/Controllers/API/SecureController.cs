using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShop.Models.Enitites;
using EShop.Services;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using EShop.ViewModels.Home;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Authorization;

namespace EShop.Controllers.API {

	[Produces("application/json")]
	[Route("api/Secure")]
	public class SecureController : Controller {
		private readonly ITelegramSender _telegram;
		private readonly DataContext _context;

		public SecureController(ITelegramSender telegram, DataContext context) {
			_telegram = telegram;
			_context = context;
		}

		// GET: api/Secure/Auth
		[Route("Auth/{code}")]
		[HttpGet]
		public IActionResult Authorize(string code) {
            return new ObjectResult( code == "pass");
        }

		// GET: api/Secure/Orders
		[HttpGet]
		[Route("Orders")]
		[Authorize]
		public IEnumerable<Order> GetOrders() {
            return _context.Orders
				.Include(o => o.Customer)
				.Include(o => o.PaymentMethod)
				.Include(o => o.ShipmentMethod)
				.Include(o => o.ShipmentMethod)
				.AsEnumerable();
        }
		
	}
}