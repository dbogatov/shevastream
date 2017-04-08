using Shevastream.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace Shevastream.Models
{
	public interface IDataContext : IDisposable
	{
		DbSet<Feedback> Feedbacks { get; set; }
		DbSet<BlogPost> BlogPosts { get; set; }
		DbSet<Order> Orders { get; set; }
		DbSet<OrderProduct> OrderProducts { get; set; }
		DbSet<Product> Products { get; set; }
		DbSet<User> Users { get; set; }

		DatabaseFacade Database { get; }

		Task<int> SaveChangesAsync(CancellationToken token = default(CancellationToken));
		int SaveChanges();
	}

	public class DataContext : DbContext, IDataContext
	{
		public DbSet<Feedback> Feedbacks { get; set; }
		public DbSet<BlogPost> BlogPosts { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderProduct> OrderProducts { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<User> Users { get; set; }

		public DataContext(DbContextOptions options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
		}
	}
}
