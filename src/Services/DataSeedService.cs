using System.Collections.Generic;
using System.Linq;
using Shevastream.Models.Entities;
using Newtonsoft.Json;
using Shevastream.Extensions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using Shevastream.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Shevastream.Services
{
	public interface IDataSeedService
	{
		/// <summary>
		/// Populates data provider with initial set of records (like enums)
		/// </summary>
		Task SeedDataAsync();

		/// <summary>
		/// A synchronous version of SeedDataAsync.
		/// Does NOT populate data points.
		/// Should be used for testing.
		/// </summary>
		void SeedData();
	}

	public class DataSeedService : IDataSeedService
	{
		private readonly IConfiguration _configuration;
		private readonly IDataContext _context;
		private readonly ICryptoService _crypto;
		private readonly IBlogService _blog;
		private readonly ILogger<DataSeedService> _logger;

		private IEnumerable<User> _users;
		private IEnumerable<Product> _products;
		private IEnumerable<BlogPost> _blogPosts;
		private IEnumerable<Order> _orders;
		private IEnumerable<OrderProduct> _orderProducts;

		public DataSeedService(
			IConfiguration configuration,
			IDataContext context, 
			ICryptoService crypto,
			IBlogService blog,
			ILogger<DataSeedService> logger)
		{
			_configuration = configuration;
			_context = context;
			_crypto = crypto;
			_blog = blog;
			_logger = logger;

			ReadConfiguration();
		}

		public void SeedData()
		{
			_logger.LogInformation(LoggingEvents.Startup.AsInt(), "DataSeed started");

			SeedSpecificEntity(_products, _context.Products);
			SeedSpecificEntity(_users, _context.Users);
			// SeedSpecificEntity(_orders, _context.Orders);
			// SeedSpecificEntity(_orderProducts, _context.OrderProducts);
			SeedSpecificEntity(_blogPosts, _context.BlogPosts);

			_logger.LogInformation(LoggingEvents.Startup.AsInt(), "DataSeed started");
		}

		public async Task SeedDataAsync()
		{
			await SeedSpecificEntityAsync(_products, _context.Products);
			await SeedSpecificEntityAsync(_users, _context.Users);
			await SeedSpecificEntityAsync(_orders, _context.Orders);
			await SeedSpecificEntityAsync(_orderProducts, _context.OrderProducts);
			await SeedSpecificEntityAsync(_blogPosts, _context.BlogPosts);
		}

		private void ReadConfiguration()
		{
			_users = _configuration
				.SectionsFromArray("Data:Users")
				.Select(section => new User {
					Id = Convert.ToInt32(section["Id"]),
					FullName = section["FullName"],
					NickName = section["NickName"],
					PassHash = _crypto.CalculateHash(section["Password"]),
					ImageUrl = section["ImageUrl"],
					Position = section["Position"],
					Occupation = section["Occupation"]
				});			

			_products = _configuration
				.SectionsFromArray("Data:Products")
				.Select(section => new Product {
					Id = Convert.ToInt32(section["Id"]),
					Name = section["Name"],
					Cost = Convert.ToInt32(section["Cost"]),
					ImageUrls = JsonConvert.SerializeObject(
						section.StringsFromArray("ImageUrls")
					),
					Description = section["Description"],
					Characteristics = JsonConvert.SerializeObject(
						section.StringsFromArray("Characteristics")
					),
					Information = section["Information"],
					VideoData = JsonConvert.SerializeObject(new {
						HasVideo = Convert.ToBoolean(section["VideoData:HasVideo"]),
						Url = section["VideoData:Url"]
					})
				});

			var blogSection = _configuration.GetSection("Data:PrivacyPolicy");
			_blogPosts = new List<BlogPost> {
				new BlogPost {
					Id = Convert.ToInt32(blogSection["Id"]),
					Author = _users.First(u => u.Id == Convert.ToInt32(blogSection["AuthorId"])),
					Active = true,
					DatePosted = Convert.ToDateTime(blogSection["DatePosted"]),
					DateUpdated = DateTime.Now,
					Title = blogSection["Title"],
					TitleUrl = _blog.GenerateUrlFromTitle(blogSection["Title"]),
					Content = blogSection["Content"],
					Preview = _blog.GeneratePreview(blogSection["Content"]),
					Views = (DateTime.Now - Convert.ToDateTime(blogSection["DatePosted"])).Days / 2
				}
			};
		}

		/// <summary>
		/// A synchronous version of SeedSpecificEntityAsync.
		/// </summary>
		private bool SeedSpecificEntity<T>(IEnumerable<T> values, DbSet<T> dbSets) where T : class
		{
			if (values.Count() != dbSets.Count())
			{
				dbSets.Clear();
				_context.SaveChanges();
				dbSets.AddRange(values);
				_context.SaveChanges();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Replace the existing entities of the T type with the supplied one.
		/// As optimization, it does it only if the number of records in the data provider is different from the number
		/// in the supplied list.
		/// </summary>
		/// <param name="values">List of supplied values to be inserted/replaced</param>
		/// <param name="dbSets">The set of entities of this type in the data provider. 
		/// These are to be replaced by values.</param>
		/// <returns>True if values has been replaced, false if values were in sync and did not require 
		/// replacement.</returns>
		private async Task<bool> SeedSpecificEntityAsync<T>(IEnumerable<T> values, DbSet<T> dbSets, bool ignoreCount = false) where T : class
		{
			if (values.Count() != await dbSets.CountAsync() || ignoreCount)
			{
				if (!ignoreCount)
				{
					dbSets.Clear();
				}
				await _context.SaveChangesAsync();
				await dbSets.AddRangeAsync(values);
				await _context.SaveChangesAsync();
				return true;
			}
			return false;
		}
	}
}
