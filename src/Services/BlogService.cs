using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Shevastream.Models.Entities;
using Shevastream.ViewModels.Blog;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Shevastream.Models;

namespace Shevastream.Services
{
	public interface IBlogService
    {
        string MarkDownToHtml(string content);
        string GenerateUrlFromTitle(string title);
        Task<IEnumerable<BlogPostViewModel>> GetAllPostsAsync();
        Task<BlogPostViewModel> GetPostByIdAsync(int id, bool active = true);
        Task<BlogPost> UpdatePostAsync(BlogPostViewModel post);
        Task<BlogPost> CreatePostAsync(BlogPostViewModel post);
        Task AddViewAsync(BlogPostViewModel post);
        Task<IEnumerable<BlogPostViewModel>> GetLatestPostsAsync(int postsNum);
    }

    public class BlogService : IBlogService
    {
        private readonly IDataContext _context;
        private readonly IAuthService _auth;
		private readonly ITransliterationService _translit;

		public BlogService(IDataContext context, IAuthService auth, ITransliterationService translit)
        {
            _auth = auth;
            _context = context;
			_translit = translit;
        }
		
        public string GenerateUrlFromTitle(string title)
        {
			title = _translit.CyrillicToLatin(title);

            // make it all lower case
            title = title.ToLower();

            // remove entities
            title = Regex.Replace(title, @"&\w+;", "");

            // remove anything that is not letters, numbers, dash, or space
            title = Regex.Replace(title, @"[^a-z0-9\-\s]", "");

            // replace spaces
            title = title.Replace(' ', '-');

            // collapse dashes
            title = Regex.Replace(title, @"-{2,}", "-");

            // trim excessive dashes at the beginning
            title = title.TrimStart(new[] { '-' });

            // if it's too long, clip it
            if (title.Length > 80)
            {
                title = title.Substring(0, 79);
            }

            // remove trailing dashes
            title = title.TrimEnd(new[] { '-' });

            return title;
        }

        public string MarkDownToHtml(string content)
        {
            return CommonMark.CommonMarkConverter.Convert(content);
        }

        public async Task<IEnumerable<BlogPostViewModel>> GetAllPostsAsync()
        {
            return await _context
                .BlogPosts
                .Include(bp => bp.Author)
                .OrderByDescending(bp => bp.DatePosted)
                .Select(bp => new BlogPostViewModel
                {
                    Id = bp.Id,
                    AuthorName = bp.Author.NickName,
                    DatePosted = bp.DatePosted,
                    Title = bp.Title,
                    TitleUrl = bp.TitleUrl,
                    Active = bp.Active,
                    Views = bp.Views,
                    Preview = bp.Preview
                })
				.ToListAsync();
        }

        public async Task<BlogPostViewModel> GetPostByIdAsync(int id, bool active = true)
        {
            if (_context.BlogPosts.Any(bp => (active ? bp.Active : true) && bp.Id == id))
            {
                var post =
                    BlogPostViewModel.FromBlogPost(await _context
                    .BlogPosts.Include(bp => bp.Author)
                    .FirstAsync(
                        bp => bp.Id == id
                    ));

                post.HtmlContent = MarkDownToHtml(post.Content);

                return post;
            }
            else
            {
                return null;
            }
        }

        public async Task<BlogPost> UpdatePostAsync(BlogPostViewModel post)
        {
            if (_context.BlogPosts.Any(bp => bp.Id == post.Id))
            {
				var user = await _auth.GetCurrentUser();

                var old = _context.BlogPosts.First(bp => bp.Id == post.Id);
                old.Title = post.Title.Trim();
                old.TitleUrl = GenerateUrlFromTitle(post.Title);
                old.Content = post.Content;
                old.Preview = MarkDownToHtml(
					post.Content.TrimStart().IndexOf('\n') > 0 ? 
					post.Content.Substring(0, post.Content.TrimStart().IndexOf('\n')) : 
					post.Content
				);
                old.DateUpdated = DateTime.Now;
                old.Author = user;
                old.Active = post.Active;

                await _context.SaveChangesAsync();

                return old;
            }

            return null;
        }

        public async Task<BlogPost> CreatePostAsync(BlogPostViewModel post)
        {
            var user = await _auth.GetCurrentUser();
			
            var @new = new BlogPost
            {
                Author = user,
                Active = post.Active,
                DatePosted = DateTime.Now,
                DateUpdated = DateTime.Now,
                Title = post.Title.Trim(),
                TitleUrl = GenerateUrlFromTitle(post.Title),
				Preview = MarkDownToHtml(
					post.Content.TrimStart().IndexOf('\n') > 0 ? 
					post.Content.Substring(0, post.Content.TrimStart().IndexOf('\n')) : 
					post.Content
				),
                Content = post.Content
            };
            _context.BlogPosts.Add(@new);

            await _context.SaveChangesAsync();

            return @new;
        }

        public async Task AddViewAsync(BlogPostViewModel post)
        {
            if (_context.BlogPosts.Any(bp => bp.Id == post.Id))
            {
                var old = _context.BlogPosts.First(bp => bp.Id == post.Id);
                old.Views++;

                await _context.SaveChangesAsync();
            }
        }

		public async Task<IEnumerable<BlogPostViewModel>> GetLatestPostsAsync(int postsNum = 3)
		{
            return (await _context
				.BlogPosts
				.Where(bp => bp.Active)
				.OrderByDescending(bp => bp.DatePosted)
				.Take(postsNum)
				.ToListAsync())
				.Select(bp => BlogPostViewModel.FromBlogPost(bp));
        }
    }
}

