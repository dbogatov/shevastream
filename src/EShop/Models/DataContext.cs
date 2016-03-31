using System;
using System.Linq;
using EShop.Models.Enitites;
using EShop.Services;
using Microsoft.Data.Entity;
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

	private readonly ICryptoService _crypto;

	public DataContext(ICryptoService crypto)
	{
		_crypto = crypto;
	}

	// This method connects the context with the database
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		//optionsBuilder.UseInMemoryDatabase();
		optionsBuilder.UseNpgsql(DataContext.connectionString);
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
        builder.HasDefaultSchema("shevastream");
        base.OnModelCreating(builder);
	}

	public void EnsureSeedData()
	{

		if (!this.ShipmentMethods.Any())
		{
			this.AddRange(
				new ShipmentMethod { Name = "К корпусу Шевченка", Cost = 0, CostTBD = false },
				new ShipmentMethod { Name = "По Киеву", Cost = 30, CostTBD = false },
				new ShipmentMethod { Name = "Новой Почтой", Cost = 0, CostTBD = true }
			);
		}

		if (!this.PaymentMethods.Any())
		{
			this.AddRange(
				new PaymentMethod { Name = "Наличными" },
				new PaymentMethod { Name = "Через систему \"Приват 24\"" }
			);
		}

		if (!this.Products.Any())
		{
			this.Add(new Product { Name = "Блокнот", Cost = 170, ImageUrls = JsonConvert.SerializeObject(new string[] { "#" }), Description = "Desc", Characteristics = "Char" });
		}

		if (!this.Users.Any())
		{
			this.AddRange(
				new User { FullName = "Dmytro Bogatov", NickName = "@dmytro", PassHash = _crypto.CalculateHash("Doomsday") },
				new User { FullName = "Polina Guley", NickName = "@polly", PassHash = _crypto.CalculateHash("cacadoo13") },
				new User { FullName = "Anton Melnikov", NickName = "@melnikov", PassHash = _crypto.CalculateHash("simplestPossiblePassword123") }
			);
		}

		if (!this.OrderStatuses.Any())
		{
			this.AddRange(
				new OrderStatus { Description = "Received" },
				new OrderStatus { Description = "In progress" },
				new OrderStatus { Description = "Waiting for shipment" },
				new OrderStatus { Description = "Done" }
			);
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