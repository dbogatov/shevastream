using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShop.Models.Enitites;
using EShop.Services;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using EShop.ViewModels.Home;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Authorization;

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