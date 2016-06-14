using Microsoft.AspNetCore.Mvc;
using EShop.Services;
using EShop.ViewModels.Blog;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace EShop.Controllers.API
{

    [Produces("application/json")]
	[Route("api/Blog")]
	public class BlogController : Controller
	{
        private readonly IBlogService _blog;
        private readonly DataContext _context;

        public BlogController(IBlogService blog, DataContext context)
		{
            _blog = blog;
            _context = context;
        }

		// POST api/Blog
		[HttpPost]
		[Authorize]
		public void Post(BlogPostViewModel post)
		{
			_blog.UpdatePostAsync(post);
		}
		
		// POST api/Blog/Activate
		[HttpPost]
		[Authorize]
		[Route("Activate")]
		public void Activate(BlogPostViewModel post)
		{
			_blog.TogglePublish(post, true);
		}
		
		// POST api/Blog/Deactivate
		[HttpPost]
		[Authorize]
		[Route("Deactivate")]
		public void Deactivate(BlogPostViewModel post)
		{
			_blog.TogglePublish(post, false);
		}

		// PUT api/Blog
		[HttpPut]
		[Authorize]
		public string Put(BlogPostViewModel post)
		{
            return JsonConvert.SerializeObject(_blog.CreatePostAsync(post).Result.TitleUrl);
        }
		
		// DELETE api/Blog
		[HttpDelete]
		[Authorize]
		public void Delete(BlogPostViewModel post)
		{
			_blog.RemovePost(post);
        }
	}
}