using System;
using System.Collections.Generic;
using System.Linq;
using EShop.Models.Enitites;
using EShop.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public class DataContext : DbContext
{
	public static string connectionString;

	public DbSet<Feedback> Feedbacks { get; set; }

	public DbSet<LogEntry> LogEntries { get; set; }

	public DbSet<Order> Orders { get; set; }
	public DbSet<ShipmentMethod> ShipmentMethods { get; set; }
	public DbSet<Customer> Customers { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<PaymentMethod> PaymentMethods { get; set; }
	public DbSet<User> Users { get; set; }
	public DbSet<OrderStatus> OrderStatuses { get; set; }

	public DbSet<PushPair> PushPairs { get; set; }

	private readonly ICryptoService _crypto;

	public DataContext(ICryptoService crypto)
	{
		_crypto = crypto;
	}

	// This method connects the context with the database
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseInMemoryDatabase();
		//optionsBuilder.UseNpgsql(DataContext.connectionString);
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.HasDefaultSchema("shevastream");
		base.OnModelCreating(builder);
	}

	public void EnsureSeedData()
	{

		var shipmentMethods = new List<ShipmentMethod> {
			new ShipmentMethod { Id = 1, Name = "К корпусу Шевченка", Cost = 0, CostTBD = false },
			new ShipmentMethod { Id = 2, Name = "По Киеву", Cost = 30, CostTBD = false },
			new ShipmentMethod { Id = 3, Name = "Новой Почтой", Cost = 0, CostTBD = true }
		};

		var paymentMethods = new List<PaymentMethod> {
			new PaymentMethod { Id = 1, Name = "Наличными" },
			new PaymentMethod { Id = 2, Name = "Через систему \"Приват 24\"" }
		};

		var products = new List<Product> {
			new Product { Id = 1, Name = "Блокнот", Cost = 185, ImageUrls = JsonConvert.SerializeObject(new string[] { "#" }), Description = "Desc", Characteristics = "Char" }
		};

		var users = new List<User> {
			new User { Id = 1, FullName = "Dmytro Bogatov", NickName = "@dmytro", PassHash = _crypto.CalculateHash("Doomsday"), ImageUrl = "https://shevastream.com/images/team/Dmytro.png" },
			new User { Id = 2, FullName = "Polina Guley", NickName = "@polly", PassHash = _crypto.CalculateHash("cacadoo13"), ImageUrl = "https://shevastream.com/images/team/Polina.png" },
			new User { Id = 3, FullName = "Anton Melnikov", NickName = "@melnikov", PassHash = _crypto.CalculateHash("simplestPossiblePassword123"), ImageUrl = "https://shevastream.com/images/team/Anton.jpg" },
			new User { Id = 4, FullName = "Taras Shevchenko", NickName = "@none", PassHash = _crypto.CalculateHash("cabooom45"), ImageUrl = "#" }
		};

		var orderStatuses = new List<OrderStatus> {
			new OrderStatus { Id = 1, Description = "Received" },
			new OrderStatus { Id = 2, Description = "In progress" },
			new OrderStatus { Id = 3, Description = "Waiting for shipment" },
			new OrderStatus { Id = 4, Description = "Done" }
		};

		if (this.ShipmentMethods.Count() != shipmentMethods.Count())
		{
			this.ShipmentMethods.Clear();
			this.ShipmentMethods.AddRange(shipmentMethods);
		}

		if (this.PaymentMethods.Count() != paymentMethods.Count())
		{
			this.PaymentMethods.Clear();
			this.PaymentMethods.AddRange(paymentMethods);
		}

		if (this.Products.Count() != products.Count())
		{
			this.Products.Clear();
			this.Products.AddRange(products);
		}

		if (this.Users.Count() != users.Count())
		{
			this.Users.Clear();
			this.Users.AddRange(users);
		}

		if (this.OrderStatuses.Count() != orderStatuses.Count())
		{
			this.OrderStatuses.Clear();
			this.OrderStatuses.AddRange(orderStatuses);
		}

		if (!this.Customers.Any())
		{
			this.AddRange(
				new Customer
				{
					Name = "Dmytro",
					Phone = "(050) 866-22-22",
					Email = "dbogatov@wpi.edu"
				}
			);
		}

		this.SaveChanges();

		if (!this.Orders.Any())
		{
			this.AddRange(
				new Order
				{
					Quantity = 2,
					Address = "100 Institute Road",
					AssigneeId = this.Users.FirstOrDefault().Id,
					OrderStatusId = this.OrderStatuses.FirstOrDefault().Id,
					ProductId = this.Products.FirstOrDefault().Id,
					CustomerId = this.Customers.FirstOrDefault().Id,
					ShipmentMethodId = this.ShipmentMethods.FirstOrDefault().Id,
					PaymentMethodId = this.PaymentMethods.FirstOrDefault().Id,
					Comment = "Wanted to try blue one",
					AssigneeComment = "Got it",
					DateCreated = DateTime.Now.Ticks,
					DateLastModified = DateTime.Now.Ticks
				}
			);
		}

		this.SaveChanges();

	}

}