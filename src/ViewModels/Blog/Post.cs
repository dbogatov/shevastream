using Shevastream.Models.Enitites;

namespace Shevastream.ViewModels.Blog
{
	public class BlogPostViewModel : BlogPost
	{
		public string HtmlContent { get; set; }
		public string AuthorName { get; set; }

		public static BlogPostViewModel FromBlogPost(BlogPost post)
		{
			return new BlogPostViewModel
			{
				Id = post.Id,
				AuthorId = post.AuthorId,
				Active = post.Active,
				DatePosted = post.DatePosted,
				DateUpdated = post.DateUpdated,
				Title = post.Title,
				TitleUrl = post.TitleUrl,
				Content = post.Content,
				Preview = post.Preview,
				Views = post.Views,
				Author = post.Author,
				HtmlContent = null,
				AuthorName = null
			};
		}
	}
}