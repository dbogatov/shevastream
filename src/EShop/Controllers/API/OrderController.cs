using System;
using System.Collections.Generic;
using System.Linq;
using EShop.Models.Enitites;
using EShop.Services;
using Microsoft.AspNetCore.Mvc;
using EShop.ViewModels.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EShop.Controllers.API
{

	[Produces("application/json")]
	[Route("api/Order")]
	public class OrderController : Controller
	{
		private readonly string CART_COOKIE_NAME = "Cart";
		private readonly string USER_COOKIE_NAME = "UserOrderData";

		private readonly ITelegramSender _telegram;
		private readonly DataContext _context;
		private readonly IDBLogService _log;
		private readonly IHostingEnvironment _env;
		private readonly IPushService _push;
		private readonly HttpContext _http;

		public OrderController(
			ITelegramSender telegram,
			DataContext context,
			IDBLogService log,
			IHostingEnvironment env,
			IPushService push,
			IHttpContextAccessor http
			)
		{
			_telegram = telegram;
			_context = context;
			_log = log;
			_env = env;
			_push = push;
			_http = http.HttpContext;
		}

		// PUT api/order
		[HttpPut]
		public void Put(OrderViewModel order)
		{
			// if cookie is not set than return immediately
			if (_http.Request.Cookies[CART_COOKIE_NAME] == null)
			{
				return;
			}

			// save user data in cookie
			_http.
				Response.
				Cookies.
				Append(
					USER_COOKIE_NAME,
					JsonConvert.SerializeObject(new OrderUserData {
						Name = order.CustomerName,
						Email = order.CustomerEmail,
						Address = order.Address,
						Phone = order.CustomerPhone
					})
				);

			// get full cart and total cost
			order.Cart = new FullCartViewModel();

			var elements = JsonConvert.
				DeserializeObject<CartViewModel>(
					_http.Request.Cookies[CART_COOKIE_NAME]
				).Elements;

			if (elements.Count == 0)
			{
				return;
			}

			var products = _context.Products.AsEnumerable();

			order.Cart.Products = (
				from element in elements
				join prod in products on element.ProductId equals prod.Id
				select new FullCartElementViewModel
				{
					Product = prod,
					Quantity = element.Quantity
				}).ToList();

			order.TotalAmountDue = order.Cart.GetTotalCost();

            // empty cart
            _http.Response.Cookies.Delete(CART_COOKIE_NAME);

            // add to database
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
					ProductId = 1, // TODO remove
					OrderStatusId = 1, // Received
					Quantity = 1, // TODO remove
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

				foreach (var item in order.Cart.Products)
				{
					_context.OrderProducts.Add(new OrderProduct
					{
						OrderId = dbOrder.Id,
						ProductId = item.Product.Id,
						Quantity = item.Quantity
					});
				}
				_context.SaveChanges();
			}
			catch (System.Exception e)
			{
				Console.WriteLine(e.StackTrace);
				_telegram.SendMessageAsync($"WARNING: the order from {order.CustomerName} - {order.CustomerPhone} has NOT been added to the database!");
			}
			
			// notify us
			_telegram.SendMessageAsync(order.ToString());

			_push.SendAll($"New order from {order.CustomerName}");
		}

		// GET: api/order
		[HttpGet]
		[Authorize]
		public IEnumerable<object> GetOrders()
		{
			return _context.Orders
				.Include(o => o.Customer)
				.Include(o => o.PaymentMethod)
				.Include(o => o.ShipmentMethod)
				.Include(o => o.ShipmentMethod)
				.Include(o => o.Assignee)
				.Include(o => o.OrderStatus)
				.Include(o => o.Product)
				.Select(o => new
				{
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