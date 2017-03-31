using System;
using System.Linq;
using System.Threading.Tasks;
using Shevastream.Models.Entities;
using Shevastream.ViewModels.Store;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Shevastream.Models;

namespace Shevastream.Services
{
	public interface IOrderService
	{
		Task<int> PutOrderAsync(OrderViewModel order);
	}

	public class OrderService : IOrderService
	{
		private readonly string USER_COOKIE_NAME = "UserOrderData";

		private readonly IDataContext _context;
		private readonly IPushService _push;
		private readonly HttpContext _http;
		private readonly ICartService _cart;

		public OrderService(
			IDataContext context,
			IPushService push,
			IHttpContextAccessor http,
			ICartService cart
			)
		{
			_context = context;
			_push = push;
			_http = http.HttpContext;
			_cart = cart;
		}

		public async Task<int> PutOrderAsync(OrderViewModel order)
		{
			if (_cart.IsCartEmpty())
			{
				return -1;
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

			var total = order.Cart.GetTotalCost();

			// empty cart
			_cart.EmptyCart();

			var dbOrder = new Order();

			// add to database
			try
			{
				dbOrder = new Order
				{
					CustomerName = order.CustomerName,
					CustomerPhone = order.CustomerPhone,
					CustomerEmail = order.CustomerEmail,
					ShipmentMethod = order.ShipmentMethod,
					Address = order.Address,
					PaymentMethod = order.PaymentMethod,
					Comment = order.Comment,
				};
				await _context.Orders.AddAsync(dbOrder);

				await _context.SaveChangesAsync();

				foreach (var item in order.Cart.Products)
				{
					// TODO use navigation property
					await _context.OrderProducts.AddAsync(new OrderProduct
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
			await _push.SendOrderAsync(order.ToString(), order.CustomerName, order.CustomerEmail, order.Cart.Products.Select(p => p.Product));

			return dbOrder.Id;
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
