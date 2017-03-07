using System;
using System.Linq;
using Shevastream.Models.Enitites;
using Shevastream.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shevastream.Controllers.API
{

	[Produces("application/json")]
	[Route("api/Push")]
	public class PushController : Controller
	{
		private readonly DataContext _context;

		public PushController(DataContext context)
		{
			_context = context;
		}

		// POST: api/RegisterDevice
		[HttpPost]
		[Route("RegisterDevice")]
		[Authorize]
		public bool RegisterDevice(RegisterDiviceViewModel model)
		{
			try
			{
				var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

				//Console.WriteLine($"id: {userId}");

				_context.PushPairs.Add(new PushPair
				{
					DeviceToken = model.DeviceToken,
					UserId = userId
				});

				_context.SaveChanges();
			}
			catch (System.Exception e)
			{
				Console.WriteLine($"Exc: {e.Message}");
				return false;
			}

			return true;
		}

		// POST: api/UnregisterDevice
		[HttpPost]
		[Route("UnregisterDevice")]
		[Authorize]
		public bool UnregisterDevice(RegisterDiviceViewModel model)
		{
			try
			{
				var pair = _context.PushPairs.FirstOrDefault(pp => pp.DeviceToken == model.DeviceToken);
				if (pair != null)
				{
					_context.PushPairs.Remove(pair);
				}

				_context.SaveChanges();
			}
			catch (System.Exception)
			{
				return false;
			}

			return true;
		}

	}
}