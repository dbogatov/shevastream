using System.Collections.Generic;
using System.Linq;
using EShop.Models.Enitites;
using Microsoft.AspNet.Mvc;

namespace EShop.Controllers.API
{

	[Produces("application/json")]
	[Route("api/Log")]
	public class LogController : Controller
	{
		private readonly DataContext _context;

		public LogController(DataContext context)
		{
			_context = context;
		}

		// GET: api/Log
		[HttpGet]
		//[Authorize]
		public IEnumerable<LogEntry> GetLogs()
		{
			return _context.LogEntries.AsEnumerable();
		}

	}
}