using Shevastream.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Shevastream.Controllers.View
{
	/// <summary>
	/// Controller responsible for error endpoints - /error
	/// </summary>
	public class ErrorController : Controller
	{
		/// <summary>
		/// Show error page for specific code
		/// </summary>
		/// <param name="code">HTTP error code</param>
		public IActionResult Error(int? code)
		{
			return View(new ErrorViewModel(code.HasValue ? code.Value : 404));
		}
	}
}
