using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shevastream.ViewModels.Store;

namespace Shevastream.Controllers
{
    public partial class ApiController
	{
		[HttpPost]
		public async Task<IActionResult> CallbackRequest(CallMeBackViewModel model)
		{
			await _push.SendCallbackAsync(model.Phone);

			return Ok("Callback request received");
		}
	}
}
