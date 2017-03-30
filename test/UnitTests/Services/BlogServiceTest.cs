using System;
using Xunit;
using Shevastream.Models.Entities;
using Shevastream.Services;
using Shevastream.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Moq;
using Microsoft.AspNetCore.Http;

namespace Shevastream.Tests.UnitTests.Services
{
	/// <summary>
	/// Unit tests for IDataSeedService
	/// </summary>
	public class BlogServiceTest
	{
		/// <summary>
		/// Provides registered service through dependency injection.
		/// </summary>
		private readonly IServiceProvider _serviceProvider;

		public BlogServiceTest()
		{
			_serviceProvider = Extensions.RegisterServices().BuildServiceProvider();
		}

		[Fact]
		public void GeneratesMarkdown()
		{
			// Arrange
			var blogService = new BlogService(
				new Mock<IDataContext>().Object,
				new Mock<IHttpContextAccessor>().Object,
				new Mock<ITransliterationService>().Object
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
			Assert.Contains("<h3>Title</h3>", markdown);
			Assert.Contains("<p>Regular</p>", markdown);
			Assert.Contains("<strong>Bold</strong>", markdown);
			Assert.Contains("<em>Italic</em>", markdown);
			Assert.Contains("<code>code</code>", markdown);
			Assert.Contains("<a href=\"url\">link</a>", markdown);
			Assert.Contains("<pre><code>tabbed code</code></pre>", markdown);
			Assert.Contains("<pre><code>quotted code</code></pre>", markdown);
			Assert.Contains("<blockquote><p>quote</p></blockquote>", markdown);
			Assert.Contains("<ul><li>list-item-1</li><li>list-item-2</li></ul>", markdown);
		}
	}
}
