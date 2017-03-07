using System.Collections.Generic;
using System.Linq;
using Shevastream.Models.Enitites;
using Microsoft.AspNetCore.Mvc;

namespace Shevastream.Controllers.API
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