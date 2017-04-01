using System.Linq;
using System.Threading.Tasks;
using Shevastream.Models.Entities;
using Shevastream.ViewModels.Store;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Shevastream.Models;
using Microsoft.Extensions.Logging;
using Shevastream.Extensions;

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
		private readonly ILogger<OrderService> _logger;

		public OrderService(
			IDataContext context,
			IPushService push,
			IHttpContextAccessor http,
			ICartService cart,
			ILogger<OrderService> logger
			)
		{
			_context = context;
			_push = push;
			_http = http.HttpContext;
			_cart = cart;
			_logger = logger;
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
			catch (System.Exception exception)
			{
				_logger.LogError(
					LoggingEvents.Order.AsInt(), 
					exception, 
					$"Error saving order {dbOrder} into the data provider unavailable. See stack trace."
				);
			}
			
			await _push.SendOrderAsync(
				order.ToString(), 
				order.CustomerName, 
				order.CustomerEmail, 
				order.Cart.Products.Select(p => p.Product)
			);

			_logger.LogInformation(
				LoggingEvents.Order.AsInt(), 
				$"Order: ${dbOrder}, cart: ${order.Cart}"
			);

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
