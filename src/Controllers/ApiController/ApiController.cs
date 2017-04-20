using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shevastream.Models;
using Shevastream.Services;

namespace Shevastream.Controllers
{
	[Produces("application/json")]
	/// <summary>
	/// Controller responsible for API endpoints - /api
	/// </summary>
	public partial class ApiController : Controller
	{
		// Context for data provider
		private readonly IDataContext _context;
		private readonly IPushService _push;
		private readonly IOrderService _orders;
		private readonly ILogger<ApiController> _logger;

		public ApiController(
			IDataContext context,
			IPushService push,
			IOrderService orders,
			ILogger<ApiController> logger
		)
		{
			_context = context;
			_push = push;
			_orders = orders;
			_logger = logger;
		}
	}
}
