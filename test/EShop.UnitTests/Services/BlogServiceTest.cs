using Xunit;
using EShop.Services;
using Microsoft.AspNetCore.Http;

namespace EShop.UnitTests.Services
{
	public class BlogServiceTest
	{
		private readonly IBlogService _blog;

		public BlogServiceTest()
		{
            _blog = new BlogService(
				new DataContext(new CryptoService()),
				new HttpContextAccessor()
			);
		}

		[Fact]
		public void PassingTest()
		{
			Assert.Equal(4, Add(2, 2));
		}

		[Fact]
		public void ServicesTest()
		{
            Assert.Equal("Moe-dly`nnoe-nazvany`e", _blog.GenerateUrlFromTitleStackOverflow("Мое длинное название"));
        }

		[Fact]
		public void FailingTest()
		{
			Assert.Equal(7, Add(3, 2));
		}

		int Add(int x, int y)
		{
			return x + y;
		}
	}
}