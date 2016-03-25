using System;
using System.Collections.Generic;
using System.Linq;
using EShop.Models.Enitites;
using EShop.Services;
using Microsoft.AspNet.Mvc;
using EShop.ViewModels.Home;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Authorization;

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
					OrderStatusId = 1, // received
					Quantity = order.Quantity,
					CustomerId = customer.Id,
					ShipmentMethodId = order.ShipmentMethodId,
					Address = order.Address,
					PaymentMethodId = order.PaymentMethodId,
					Comment = order.Comment,
					DateCreated = DateTime.Now.Ticks,
					DateLastModified = DateTime.Now.Ticks,
					AssigneeComment = "Got it."
				};

				_context.Orders.Add(dbOrder);
				_context.SaveChanges();
				
				_log.LogActionAsync(DBLogEntryType.OrderReceived, dbOrder.Id);
			}
			catch (System.Exception)
			{
				_telegram.SendMessageAsync($"WARNING: the order from {order.CustomerName} ({order.CustomerEmail}) has NOT been added to the database!");
			}

			_telegram.SendMessageAsync(order.ToString());

			return true;
		}

		// GET: api/order
		[HttpGet]
		[Authorize]
		public IEnumerable<object> GetOrders()
		{
			_log.LogActionAsync(DBLogEntryType.UserPulledOrders, Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value));

			return _context.Orders
				.Include(o => o.Customer)
				.Include(o => o.PaymentMethod)
				.Include(o => o.ShipmentMethod)
				.Include(o => o.ShipmentMethod)
				.Include(o => o.Assignee)
				.Include(o => o.OrderStatus)
				.Include(o => o.Product)
				.Select(o => new {
					Id = o.Id,
					Assignee = o.Assignee.NickName,
					OrderStatus = o.OrderStatus.Description,
					Product = o.Product.Name,
					Quantity = o.Quantity,
					Customer = o.Customer.Name,
					ShipmentMethod = o.ShipmentMethod.Name,
					Address = o.Address,
					PaymentMethod = o.PaymentMethod.Name,
					Phone = o.Customer.Phone,
					Comment = o.Comment,
					AssigneeComment = o.AssigneeComment,
					DateCreated = new DateTime(o.DateCreated),
					DateLastModified = new DateTime(o.DateLastModified)
				})
				.AsEnumerable();
		}

		// POST api/order
		[HttpPost]
		[Authorize]
		[Route("AssignToSelf")]
		public bool AssignToSelf(OrderIdViewModel order)
		{
			Console.WriteLine("Taken");

			var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

			_context.Orders.FirstOrDefault(o => o.Id == order.Id).AssigneeId = userId;
			
			_context.SaveChanges();

			return true;
		}

		// POST api/order
		[HttpPost]
		[Authorize]
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