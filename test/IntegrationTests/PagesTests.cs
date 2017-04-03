using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.TestHost;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Shevastream.Models;
using Shevastream.Services.Factories;
using Xunit;

namespace Shevastream.Tests.IntegrationTests
{
	public class PagesTests
	{
		/// <summary>
		/// Server object which mimics a real running server
		/// </summary>
		private readonly TestServer _server;
		/// <summary>
		/// Client object which mimics a real client
		/// </summary>
		private readonly HttpClient _client;

		private readonly IDataContext _dataContext;

		/// <summary>
		/// Setup mock server and client
		/// </summary>
		public PagesTests()
		{
			var path = PlatformServices.Default.Application.ApplicationBasePath;
			var contentPath = Path.GetFullPath(Path.Combine(path, $@"../../../../src"));

			_server = new TestServer(
				new WebHostBuilder()
					.UseContentRoot(contentPath)
					.UseStartup<Startup>()
					.ConfigureServices(services =>
					{
						services.AddSingleton<IHttpClientFactory, HttpClientFactory>();
						services.Configure((RazorViewEngineOptions options) =>
						{
							var previous = options.CompilationCallback;
							options.CompilationCallback = (context) =>
							{
								previous?.Invoke(context);

								var assembly = typeof(Startup).GetTypeInfo().Assembly;
								var assemblies = assembly.GetReferencedAssemblies().Select(x => MetadataReference.CreateFromFile(Assembly.Load(x).Location))
								.ToList();
								assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("mscorlib")).Location));
								assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Private.Corelib")).Location));
								assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor")).Location));
								assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("mscorlib")).Location)); 
								assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Private.Corelib")).Location)); 
								assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Linq")).Location)); 
								assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Threading.Tasks")).Location)); 
								assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location)); assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Dynamic.Runtime")).Location)); 
								assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor.Runtime")).Location)); 
								assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc")).Location)); 
								assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor")).Location)); 
								assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc.Razor")).Location));
								assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Html.Abstractions")).Location)); 
								assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Text.Encodings.Web")).Location));

								context.Compilation = context.Compilation.AddReferences(assemblies);
							};
						});
					})
			);

			_client = _server.CreateClient();

			var serviceProvider = Extensions.RegisterServices().BuildServiceProvider();
			_dataContext = serviceProvider.GetRequiredService<IDataContext>();
		}

		[Theory]
		[InlineData("/home")]
		[InlineData("/home/profile")]
		[InlineData("/store/product")]
		[InlineData("/home/contact")]
		[InlineData("/store/cart")]
		[InlineData("/home/faq")]
		[InlineData("/account/login")]
		public async Task TestSimplePages(string url)
		{
			// Act
			var ok = await _client.GetAsync(url);

			// Assert
			Assert.Equal(HttpStatusCode.OK, ok.StatusCode);
		}

		[Fact]
		public async Task TestBlogPages()
		{
			// Arrange
			var posts = _dataContext.BlogPosts.AsEnumerable();

			// Act
			var results = await Task.WhenAll(
				posts
					.Select(post => _client.GetAsync($"/blog/{post.Id}/{post.TitleUrl}"))
			);

			// Assert
			results.ToList().ForEach(res => Assert.Equal(HttpStatusCode.OK, res.StatusCode));
		}

		[Fact]
		public async Task TestProductPages()
		{
			// Arrange
			var products = _dataContext.Products.AsEnumerable();

			// Act
			var results = await Task.WhenAll(
				products
					.Select(product => _client.GetAsync($"/store/product/{product.Id}"))
			);

			// Assert
			results.ToList().ForEach(res => Assert.Equal(HttpStatusCode.OK, res.StatusCode));
		}
	}
}
