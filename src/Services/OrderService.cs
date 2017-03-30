using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shevastream.Models.Entities;
using Shevastream.ViewModels.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shevastream.Models;

namespace Shevastream.Services
{
	public interface IOrderService
	{
		Task PutOrderAsync(OrderViewModel order);
		Task<IEnumerable<object>> GetOrdersAsync();
	}

	public class OrderService : IOrderService
	{
		private readonly string USER_COOKIE_NAME = "UserOrderData";

		private readonly DataContext _context;
		private readonly IHostingEnvironment _env;
		private readonly IPushService _push;
		private readonly HttpContext _http;
		private readonly ICartService _cart;

		public OrderService(
			DataContext context,
			IHostingEnvironment env,
			IPushService push,
			IHttpContextAccessor http,
			ICartService cart
			)
		{
			_context = context;
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

			var total = order.Cart.GetTotalCost();

			// empty cart
			_cart.EmptyCart();

			// add to database
			try
			{
				var dbOrder = new Order
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
		}

		public async Task<IEnumerable<object>> GetOrdersAsync()
		{
			// TODO try SelectMany or navigation properties
			var orderProducts = await _context.OrderProducts.ToListAsync();
			var products = await _context.Products.ToListAsync();
			var orders = await _context.Orders.ToListAsync();

			return orders
				.Select(o => new
				{
					o.Id,
					Products = (
						from orderProduct in orderProducts.Where(op => op.OrderId == o.Id)
						join product in products on orderProduct.ProductId equals product.Id
						select new
						{
							product.Name,
							product.Cost,
							orderProduct.Quantity
						}
					),
					o.CustomerName,
					o.CustomerEmail,
					o.CustomerPhone,
					o.ShipmentMethod,
					Address = o.Address ?? "",
					o.PaymentMethod,
					Comment = o.Comment ?? "No commnet",
					o.DateCreated
				})
				.AsEnumerable();
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
