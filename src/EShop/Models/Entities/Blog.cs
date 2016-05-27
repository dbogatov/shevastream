using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models.Enitites
{

	[Table("BlogPost")]
	public class BlogPost
	{
		[Key]
		public int Id { get; set; }
		
		public int AuthorId { get; set; }
		public bool Active { get; set; }
		public DateTime DatePosted { get; set; }
		public DateTime DateUpdated { get; set; }
		
		// [AlternateKey] Fluent API
		public string Title { get; set; }
		// [AlternateKey] Fluent API
		public string TitleUrl { get; set; }
        public string Content { get; set; }

        // navigation properties		
        [ForeignKey("AuthorId")]
		public User Author { get; set; }
    }
}