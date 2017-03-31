using System;
using System.ComponentModel.DataAnnotations;

namespace Shevastream.Models.Entities
{
	public class BlogPost
	{
		[Key]
		public int Id { get; set; }

		// public int AuthorId { get; set; }

		[Display(Name = "Publish")]
		public bool Active { get; set; }
		public DateTime DatePosted { get; set; } = DateTime.Now;
		public DateTime DateUpdated { get; set; }

		// [AlternateKey] Fluent API
		public string Title { get; set; }
		// [AlternateKey] Fluent API
		public string TitleUrl { get; set; }

		public string Content { get; set; }

		public string Preview { get; set; }

		public int Views { get; set; } = 1;

		public User Author { get; set; }
	}
}
