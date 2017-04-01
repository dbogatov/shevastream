using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Linq;
using Shevastream.Models;
using Shevastream.Models.Entities;
using Shevastream.Services;
using Xunit;
using Shevastream.Extensions;

namespace Shevastream.Tests.UnitTests.Services
{
	public class SiteMapServiceTest
	{
		private readonly IDataContext _dataContext;
		private readonly IUrlHelperFactory _urlHelperFactory;
		private readonly IActionContextAccessor _accessor;

		public SiteMapServiceTest()
		{
			var serviceProvider = Extensions.RegisterServices().BuildServiceProvider();

			var posts = new List<BlogPost>
			{
				new BlogPost { Id = 1, TitleUrl = "post-1", DateUpdated = DateTime.Now, Active = true },
				new BlogPost { Id = 2, TitleUrl = "post-2", DateUpdated = DateTime.Now, Active = false },
				new BlogPost { Id = 3, TitleUrl = "post-3", DateUpdated = DateTime.Now, Active = true }
			};

			var product = new Product { Id = 1 };

			_dataContext = serviceProvider.GetRequiredService<IDataContext>();
			_dataContext.BlogPosts.AddRange(posts);
			_dataContext.Products.Add(product);
			_dataContext.SaveChanges();

			var accessor = new Mock<IActionContextAccessor>();
			accessor
				.Setup(acc => acc.ActionContext)
				.Returns(new Mock<ActionContext>().Object);
			_accessor = accessor.Object;

			var urlHelper = new Mock<IUrlHelper>();
			urlHelper
				.Setup(helper => helper.RouteUrl(It.IsAny<UrlRouteContext>()))
				.Returns("https://domain.com");
			urlHelper
				.Setup(helper => helper.Action(It.IsAny<UrlActionContext>()))
				.Returns("https://domain.com");

			var urlHelperFactory = new Mock<IUrlHelperFactory>();
			urlHelperFactory
				.Setup(factory => factory.GetUrlHelper(It.IsAny<ActionContext>()))
				.Returns(urlHelper.Object);
			_urlHelperFactory = urlHelperFactory.Object;
		}

		[Fact]
		public void GetsSiteMap()
		{
			// Arrange
			var siteMapService = new SiteMapService(
				_dataContext,
				_urlHelperFactory,
				_accessor
			);

			// Act
			var siteMap = siteMapService.GetSiteMap();

			// Assert
			Assert.Equal(6, siteMap.Items.Count());
			Assert.Equal(2, siteMap.Items.Count(item => item.ChangeFreq == ChangeFrequency.Daily));
			Assert.Equal(1, siteMap.Items.Count(item => item.ChangeFreq == ChangeFrequency.Weekly));
			Assert.Equal(3, siteMap.Items.Count(item => item.ChangeFreq == ChangeFrequency.Monthly));
		}
	}
}

