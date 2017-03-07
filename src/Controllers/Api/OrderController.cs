using System.Collections.Generic;
using EShop.Services;
using Microsoft.AspNetCore.Mvc;
using EShop.ViewModels.Store;
using Microsoft.AspNetCore.Authorization;

namespace EShop.Controllers.API
{

    [Produces("application/json")]
	[Route("api/Order")]
	public class OrderController : Controller
	{
		private readonly IOrderService _order;

		public OrderController(IOrderService order)
		{
			_order = order;
		}

		// PUT api/order
		[HttpPut]
		public void Put(OrderViewModel order)
		{
			_order.PutOrderAsync(order);
		}

		// GET: api/order
		[HttpGet]
		[Authorize]
		public IEnumerable<object> GetOrders()
		{
			return _order.GetOrders();
		}

		// POST api/AssignToSelf
		[HttpPost]
		[Authorize]
		[Route("AssignToSelf")]
		public bool AssignToSelf(OrderIdViewModel order)
		{
			return _order.AssignToSelf(order);
		}

		// POST api/ChangeStatus
		[HttpPost]
		[Authorize]
		[Route("ChangeStatus")]
		public bool ChangeStatus(OrderStatusViewModel order)
		{
			return _order.ChangeStatus(order);
		}

		// POST api/order
		[HttpPost]
		[Authorize]
		public bool Post(OrderViewModel order)
		{
			return _order.UpdateOrder(order);
		}

	}
}