using Microsoft.AspNetCore.Mvc;
using Shevastream.Models;
using Shevastream.Services;

namespace Shevastream.Controllers
{
	[Produces("application/json")]
	public partial class ApiController : Controller
	{
		// Context for data provider
		private readonly IDataContext _context;
		private readonly IPushService _push;
		private readonly IOrderService _orders;

		public ApiController(
			IDataContext context,
			IPushService push,
			IOrderService orders
		)
		{
			_context = context;
			_push = push;
			_orders = orders;
		}
	}
}
