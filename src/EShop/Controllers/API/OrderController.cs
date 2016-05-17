using System;
using System.Collections.Generic;
using System.Linq;
using EShop.Models.Enitites;
using EShop.Services;
using Microsoft.AspNetCore.Mvc;
using EShop.ViewModels.Home;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace EShop.Controllers.API
{

	[Produces("application/json")]
	[Route("api/Order")]
	public class OrderController : Controller
	{
		private readonly ITelegramSender _telegram;
		private readonly DataContext _context;
		private readonly IDBLogService _log;
		private readonly IHostingEnvironment _env;
		private readonly IPushService _push;

		public OrderController(
			ITelegramSender telegram,
			DataContext context,
			IDBLogService log,
			IHostingEnvironment env,
			IPushService push
			)
		{
			_telegram = telegram;
			_context = context;
			_log = log;
			_env = env;
			_push = push;
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
					OrderStatusId = 1, // Received
					Quantity = order.Quantity,
					CustomerId = customer.Id,
					ShipmentMethodId = order.ShipmentMethodId,
					Address = order.Address,
					PaymentMethodId = order.PaymentMethodId,
					Comment = order.Comment,
					DateCreated = DateTime.Now.Ticks,
					DateLastModified = DateTime.Now.Ticks,
					AssigneeComment = "Got it.",
					AssigneeId = 4, // Taras (none)
				};

				_context.Orders.Add(dbOrder);
				_context.SaveChanges();
				
				//_log.LogActionAsync(DBLogEntryType.OrderReceived, dbOrder.Id);
			}
			catch (System.Exception)
			{
				_telegram.SendMessageAsync($"WARNING: the order from {order.CustomerName} ({order.CustomerEmail}) has NOT been added to the database!");
			}

			if (_env.IsProduction())
			{
				_telegram.SendMessageAsync(order.ToString());	
			}
			
			_push.SendAll($"New order from {order.CustomerName}");

			return true;
		}

		// GET: api/order
		[HttpGet]
		[Authorize]
		public IEnumerable<object> GetOrders()
		{
			//_log.LogActionAsync(DBLogEntryType.UserPulledOrders, Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value));

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
					Assignee = o.Assignee == null ? "none" : o.Assignee.NickName,
					OrderStatus = o.OrderStatus.Description,
					Product = o.Product.Name,
					Quantity = o.Quantity,
					Customer = o.Customer.Name,
					ShipmentMethod = o.ShipmentMethod.Name,
					Address = o.Address ?? "",
					PaymentMethod = o.PaymentMethod.Name,
					Phone = o.Customer.Phone,
					Comment = o.Comment ?? "No commnet",
					AssigneeComment = o.AssigneeComment,
					DateCreated = new DateTime(o.DateCreated),
					DateLastModified = new DateTime(o.DateLastModified)
				})
				.AsEnumerable();
		}

		// POST api/AssignToSelf
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
		
		// POST api/ChangeStatus
		[HttpPost]
		[Authorize]
		[Route("ChangeStatus")]
		public bool ChangeStatus(OrderStatusViewModel order)
		{
			Console.WriteLine($"New status {order.Status}");

			var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

			_context.Orders.FirstOrDefault(o => o.Id == order.Id).OrderStatusId = order.Status;
			
			try
			{
				_context.SaveChanges();
			}
			catch (System.Exception)
			{
                return false;
            }
			
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