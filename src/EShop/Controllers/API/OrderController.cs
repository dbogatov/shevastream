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
using System.Threading;
using System.Security.Claims;

namespace EShop.Controllers.API
{

	[Produces("application/json")]
	[Route("api/Order")]
	public class OrderController : Controller
	{
		private readonly ITelegramSender _telegram;
		private readonly DataContext _context;
		private readonly IDBLogService _log;

		public OrderController(
			ITelegramSender telegram,
			DataContext context,
			IDBLogService log
			)
		{
			_telegram = telegram;
			_context = context;
			_log = log;
		}

		// PUT api/order
		[HttpPut]
		public bool Put(OrderViewModel order)
		{
			try
			{
				var customer = new Customer
				{
					Name = order.CustomerName,
					Phone = order.CustomerPhone,
					Email = order.CustomerEmail
				};

				_context.Customers.Add(customer);
				_context.SaveChanges();

				var dbOrder = new Order
				{
					ProductId = order.ProductId,
					OrderStatusId = 1,
					Quantity = order.Quantity,
					CutomerId = customer.Id,
					ShipmentMethodId = order.ShipmentMethodId,
					Address = order.Address,
					PaymentMethodId = order.PaymentMethodId,
					Comment = order.Comment,
					DateCreated = DateTime.Now.Ticks,
					DateLastModified = DateTime.Now.Ticks
				};

				_context.Orders.Add(dbOrder);
				_context.SaveChanges();
				
				_log.LogActionAsync(DBLogEntryType.OrderReceived, dbOrder.Id);
			}
			catch (System.Exception)
			{
				_telegram.SendMessageAsync($"WARNING: the order from {order.CustomerName} ({order.CustomerEmail}) has NOT been added to the database!");
			}

			//_telegram.SendMessageAsync(order.ToString());

			return true;
		}

		// GET: api/order
		[HttpGet]
		//[Authorize]
		public IEnumerable<Order> GetOrders()
		{
			//Console.WriteLine($"UserID: {User.Claims.FirstOrDefault(c => c.Type == "UserId").Value}");

            //_log.LogActionAsync(DBLogEntryType.UserPulledOrders, Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value));

            return _context.Orders
				.Include(o => o.Customer)
				.Include(o => o.PaymentMethod)
				.Include(o => o.ShipmentMethod)
				.Include(o => o.ShipmentMethod)
				.Include(o => o.Assignee)
				.Include(o => o.OrderStatus)
				.Include(o => o.Product)
				.AsEnumerable();
		}

		// POST api/order
		[HttpPost]
		public bool Post(OrderViewModel order)
		{
			var dbOrder = _context.Orders.FirstOrDefault(o => o.Id == order.Id);

			if (dbOrder == null)
			{
				return false;
			}

			dbOrder.AssigneeId = order.AssigneeId;
			dbOrder.OrderStatusId = order.OrderStatusId;
			dbOrder.DateLastModified = DateTime.Now.Ticks;

			_context.SaveChanges();

			return true;
		}

	}
}