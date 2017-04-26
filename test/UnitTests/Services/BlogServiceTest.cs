using System;
using Xunit;
using Shevastream.Services;
using Shevastream.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Shevastream.Models.Entities;
using System.Threading.Tasks;
using Shevastream.ViewModels.Blog;
using Microsoft.Extensions.Logging;

namespace Shevastream.Tests.UnitTests.Services
{
	public class BlogServiceTest
	{
		private readonly IAuthService _authService;
		private readonly IDataContext _dataContext;
		private readonly ITransliterationService _translitService;

		public BlogServiceTest()
		{
			var serviceProvider = Extensions.RegisterServices().BuildServiceProvider();

			var user = new User
			{
				Id = 1,
				FullName = "Test user"
			};

			_dataContext = serviceProvider.GetRequiredService<IDataContext>();
			_dataContext.Users.Add(user);
			_dataContext.SaveChanges();


			var authMock = new Mock<IAuthService>();
			authMock.Setup(auth => auth.GetCurrentUserId()).Returns(1);
			_authService = authMock.Object;

			_translitService = serviceProvider.GetRequiredService<ITransliterationService>();
		}

		[Fact]
		public void GeneratesMarkdown()
		{
			// Arrange
			var blogService = new BlogService(
				new Mock<IDataContext>().Object,
				new Mock<IAuthService>().Object,
				new Mock<ITransliterationService>().Object,
				new Mock<ILogger<BlogService>>().Object
			);

			var text = $@"
### Title

Regular

**Bold**

*Italic*

`code`

[link](url)

	tabbed code

```
quotted code
```

> quote

* list-item-1
* list-item-2
			";

			// Act
			var markdown = blogService.MarkDownToHtml(text).Replace("\n", "");

			// Assert
			new List<string> {
				"<h3>Title</h3>",
				"<p>Regular</p>",
				"<strong>Bold</strong>",
				"<em>Italic</em>",
				"<code>code</code>",
				"<a href=\"url\">link</a>",
				"<pre><code>tabbed code</code></pre>",
				"<pre><code>quotted code</code></pre>",
				"<blockquote><p>quote</p></blockquote>",
				"<ul><li>list-item-1</li><li>list-item-2</li></ul>"
			}.ForEach(expected =>
				Assert.Contains(expected, markdown)
			);
		}

		[Fact]
		public void GeneratesProperTitleUrl()
		{
			// Arrange
			var blogService = new BlogService(
				new Mock<IDataContext>().Object,
				new Mock<IAuthService>().Object,
				_translitService,
				new Mock<ILogger<BlogService>>().Object
			);

			var expected = new Dictionary<string, string> {
				{ "title", "title" },
				{ "Title word", "title-word" },
				{ "Title word 123", "title-word-123" },
				{
					"Британія оприлюднила план заміни законів ЄС після Brexit",
					"britaniya-oprilyudnila-plan-zamini-zakoniv-ies-pislya-brexit"
				},
				{ "Авдіївка досі без світла", "avdiyivka-dosi-bez-svitla" }
			};

			// Act
			var actual = expected.ToDictionary(pair => pair.Value, pair => blogService.GenerateUrlFromTitle(pair.Key));

			// Assert
			actual.ToList().ForEach(entry =>
				Assert.Equal(entry.Key, entry.Value)
			);
		}

		[Fact]
		public async Task GetsAllPosts()
		{
			// Arrange
			var blogService = new BlogService(
				_dataContext,
				new Mock<IAuthService>().Object,
				new Mock<ITransliterationService>().Object,
				new Mock<ILogger<BlogService>>().Object
			);

			await _dataContext.BlogPosts.AddRangeAsync(new List<BlogPost> {
				new BlogPost { Id = 1, Title = "Title 1", Active = true },
				new BlogPost { Id = 2, Title = "Title 2" , Active = false }
			});
			await _dataContext.SaveChangesAsync();

			// Act
			var posts = await blogService.GetAllPostsAsync();

			// Assert
			Assert.Equal(posts.Count(), 2);
			Assert.True(posts.Any(bp => bp.Id == 1));
			Assert.True(posts.Any(bp => bp.Id == 2));
		}

		[Fact]
		public async Task GetsPostById()
		{
			// Arrange
			var blogService = new BlogService(
				_dataContext,
				new Mock<IAuthService>().Object,
				new Mock<ITransliterationService>().Object,
				new Mock<ILogger<BlogService>>().Object
			);

			await _dataContext.BlogPosts.AddRangeAsync(new List<BlogPost> {
				new BlogPost { Id = 1, Title = "Title 1", Active = true },
				new BlogPost { Id = 2, Title = "Title 2" , Active = false }
			});
			await _dataContext.SaveChangesAsync();

			// Act
			var existingActivePost = await blogService.GetPostByIdAsync(1);
			var existingInactivePost = await blogService.GetPostByIdAsync(2, false);
			var nonExistingPost = await blogService.GetPostByIdAsync(3);

			// Assert
			Assert.NotNull(existingActivePost);
			Assert.NotNull(existingInactivePost);
			Assert.Null(nonExistingPost);
		}

		[Fact]
		public async Task UpdatesPost()
		{
			// Arrange
			var translitService = new Mock<ITransliterationService>();
			translitService.Setup(tr => tr.CyrillicToLatin(It.IsAny<string>())).Returns("title-url");

			var blogService = new BlogService(
				_dataContext,
				_authService,
				translitService.Object,
				new Mock<ILogger<BlogService>>().Object
			);

			var blogPost = new BlogPost { Title = "Title", Content = "Content", Active = true };
			var dbPost = await _dataContext.BlogPosts.AddAsync(blogPost);
			await _dataContext.SaveChangesAsync();

			var goodPost = BlogPostViewModel.FromBlogPost(blogPost);
			goodPost.Id = dbPost.Entity.Id;
			goodPost.Title = "New title";

			var badPost = BlogPostViewModel.FromBlogPost(blogPost);
			badPost.Id = -1;

			// Act
			var successfull = await blogService.UpdatePostAsync(goodPost);
			var unsuccessful = await blogService.UpdatePostAsync(badPost);

			// Assert
			Assert.NotNull(successfull);
			Assert.Equal("New title", _dataContext.BlogPosts.First(bp => bp.Id == successfull.Id).Title);

			Assert.Null(unsuccessful);
		}

		[Fact]
		public async Task CreatesPost()
		{
			// Arrange
			var translitService = new Mock<ITransliterationService>();
			translitService.Setup(tr => tr.CyrillicToLatin(It.IsAny<string>())).Returns("title-url");

			var blogService = new BlogService(
				_dataContext,
				_authService,
				translitService.Object,
				new Mock<ILogger<BlogService>>().Object
			);

			var blogPost = BlogPostViewModel.FromBlogPost(
				new BlogPost { Title = "Title", Content = "Content", Active = true }
			);

			// Act
			var successfull = await blogService.UpdatePostAsync(blogPost);

			// Assert
			Assert.NotNull(successfull);
			Assert.Equal("Title", _dataContext.BlogPosts.First(bp => bp.Id == successfull.Id).Title);
		}

		[Fact]
		public async Task AddsView()
		{
			// Arrange
			var blogService = new BlogService(
				_dataContext,
				new Mock<IAuthService>().Object,
 				new Mock<ITransliterationService>().Object,
				 new Mock<ILogger<BlogService>>().Object
			);

			var blogPost = new BlogPost { Title = "Title", Content = "Content", Active = true };
			var dbPost = await _dataContext.BlogPosts.AddAsync(blogPost);
			await _dataContext.SaveChangesAsync();

			var beforeViews = dbPost.Entity.Views;

			// Act
			await blogService.AddViewAsync(BlogPostViewModel.FromBlogPost(blogPost));

			// Assert
			Assert.Equal(1, beforeViews);
			Assert.Equal(2, _dataContext.BlogPosts.First(bp => bp.Id == dbPost.Entity.Id).Views);
		}

		[Fact]
		public async Task GetsLatestPosts()
		{
			// Arrange
			var blogService = new BlogService(
				_dataContext,
				new Mock<IAuthService>().Object,
				new Mock<ITransliterationService>().Object,
				new Mock<ILogger<BlogService>>().Object
			);

			await _dataContext.BlogPosts.AddRangeAsync(new List<BlogPost> {
				new BlogPost { Title = "Title 1", Active = true, DatePosted = new DateTime(2017, 04, 10) },
				new BlogPost { Title = "Title 2" , Active = true, DatePosted = new DateTime(2017, 04, 11) },
				new BlogPost { Title = "Title 3" , Active = false, DatePosted = new DateTime(2017, 04, 12) },
				new BlogPost { Title = "Title 4" , Active = true, DatePosted = new DateTime(2017, 04, 13) }
			});
			await _dataContext.SaveChangesAsync();

			// Act
			var posts = await blogService.GetLatestPostsAsync(2);

			// Assert
			Assert.Equal(posts.Count(), 2);
			Assert.True(posts.Any(bp => bp.Title == "Title 2"));
			Assert.True(posts.Any(bp => bp.Title == "Title 4"));
		}
	}
}

