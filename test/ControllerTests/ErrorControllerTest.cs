using Microsoft.AspNetCore.Mvc;
using Xunit;
using Shevastream.Controllers.View;
using Shevastream.ViewModels;

namespace Shevastream.Tests.ControllerTests
{
	/// <summary>
	/// Integration tests for HomeController
	/// </summary>
	public partial class ErrorControllerTest
	{
		[Theory]
		[InlineData(400)]
		[InlineData(401)]
		[InlineData(404)]
		[InlineData(500)]
		[InlineData(null)]
		public void Error(int? code)
		{
			// Act
			var result = new ErrorController().Error(code);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);

			var model = Assert.IsAssignableFrom<ErrorViewModel>(
				viewResult.ViewData.Model
			);

			Assert.Equal(code ?? 404, model.Code);
		}
	}
}
