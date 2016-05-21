using EShop.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers.View
{
	public class ErrorController : Controller
	{
		public IActionResult Error(int? code)
		{
			return View(new ErrorViewModel(code.HasValue ? code.Value : 404));
		}
	}
}
