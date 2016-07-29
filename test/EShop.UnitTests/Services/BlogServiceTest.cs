using Xunit;
using EShop.Services;
using Moq;
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
            Assert.Equal(_blog.GenerateUrlFromTitleStackOverflow("Мое длинное название"), "Moje-dlinnoje-nazvanije");
        }

		[Fact]
		public void FailingTest()
		{
			Assert.Equal(5, Add(3, 2));
		}

		int Add(int x, int y)
		{
			return x + y;
		}
	}
}