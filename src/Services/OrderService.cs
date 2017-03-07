using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shevastream.Models.Enitites;
using Shevastream.ViewModels.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Shevastream.Services
{
	public interface IOrderService
	{
		Task PutOrderAsync(OrderViewModel order);
		IEnumerable<object> GetOrders();
		bool AssignToSelf(OrderIdViewModel order);
		bool ChangeStatus(OrderStatusViewModel order);
		bool UpdateOrder(OrderViewModel order);
	}

	public class OrderService : IOrderService
	{
		private readonly string USER_COOKIE_NAME = "UserOrderData";

		private readonly DataContext _context;
		private readonly IDBLogService _log;
		private readonly IHostingEnvironment _env;
		private readonly IPushService _push;
		private readonly HttpContext _http;
		private readonly ICartService _cart;

		public OrderService(
			DataContext context,
			IDBLogService log,
			IHostingEnvironment env,
			IPushService push,
			IHttpContextAccessor http,
			ICartService cart
			)
		{
			_context = context;
			_log = log;
			_env = env;
			_push = push;
			_http = http.HttpContext;
			_cart = cart;
		}

		public async Task PutOrderAsync(OrderViewModel order)
		{
			if (_cart.IsCartEmpty())
			{
				return;
			}

			SaveUserData(
				new OrderUserData
				{
					Name = order.CustomerName,
					Email = order.CustomerEmail,
					Address = order.Address,
					Phone = order.CustomerPhone
				}
			);

			// get full cart and total cost
			order.Cart = _cart.GetCart();

			order.TotalAmountDue = order.Cart.GetTotalCost();

			// empty cart
			_cart.EmptyCart();

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
				await _context.SaveChangesAsync();

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
				await _context.SaveChangesAsync();

				foreach (var item in order.Cart.Products)
				{
					_context.OrderProducts.Add(new OrderProduct
					{
						OrderId = dbOrder.Id,
						ProductId = item.Product.Id,
						Quantity = item.Quantity
					});
				}
				await _context.SaveChangesAsync();
			}
			catch (System.Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}
			// notify them
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			_push.SendOrderAsync(order.ToString(), order.CustomerName, order.CustomerEmail, order.Cart.Products.Select(p => p.Product));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

			_push.SendAll($"New order from {order.CustomerName}");
		}

		public IEnumerable<object> GetOrders()
		{
			var orderProducts = _context.OrderProducts.ToList();
			var products = _context.Products.ToList();

			return _context.Orders
				.Include(o => o.Customer)
				.Include(o => o.PaymentMethod)
				.Include(o => o.ShipmentMethod)
				.Include(o => o.ShipmentMethod)
				.Include(o => o.Assignee)
				.Include(o => o.OrderStatus)
				.Include(o => o.Product)
				.ToList()
				.Select(o => new
				{
					Id = o.Id,
					Assignee = o.Assignee == null ? "none" : o.Assignee.NickName,
					OrderStatus = o.OrderStatus.Description,
					Products = (
						from orderProduct in orderProducts.Where(op => op.OrderId == o.Id)
						join product in products on orderProduct.ProductId equals product.Id
						select new
						{
							ProductName = product.Name,
							Cost = product.Cost,
							Quantity = orderProduct.Quantity
						}
					),
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

		public bool AssignToSelf(OrderIdViewModel order)
		{
			Console.WriteLine("Taken");

			var userId = Convert.ToInt32(_http.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

			_context.Orders.FirstOrDefault(o => o.Id == order.Id).AssigneeId = userId;

			_context.SaveChanges();

			return true;
		}

		public bool ChangeStatus(OrderStatusViewModel order)
		{
			Console.WriteLine($"New status {order.Status}");

			var userId = Convert.ToInt32(_http.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

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

		public bool UpdateOrder(OrderViewModel order)
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

		private void SaveUserData(OrderUserData data)
		{
			// save user data in cookie
			_http.
				Response.
				Cookies.
				Append(
					USER_COOKIE_NAME,
					JsonConvert.SerializeObject(data)
				);
		}
	}
}
