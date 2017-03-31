using Shevastream.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;

namespace Shevastream.Models
{
	public interface IDataContext
	{
		DbSet<Feedback> Feedbacks { get; set; }
		DbSet<BlogPost> BlogPosts { get; set; }
		DbSet<Order> Orders { get; set; }
		DbSet<OrderProduct> OrderProducts { get; set; }
		DbSet<Product> Products { get; set; }
		DbSet<User> Users { get; set; }

		Task<int> SaveChangesAsync(CancellationToken token = default(CancellationToken));
		int SaveChanges();
	}

	public class DataContext : DbContext, IDataContext
	{
		public static string connectionString;
		public static string version;

		public DbSet<Feedback> Feedbacks { get; set; }
		public DbSet<BlogPost> BlogPosts { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderProduct> OrderProducts { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<User> Users { get; set; }

		public DataContext(DbContextOptions options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.HasDefaultSchema("shevastream");

			base.OnModelCreating(builder);
		}
	}
}
