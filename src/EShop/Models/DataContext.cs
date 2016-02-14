using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShop.Models.Enitites;
using Microsoft.Data.Entity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

public class DataContext : DbContext
{
	// This property defines the table
	public DbSet<Feedback> Feedbacks { get; set; }

	public DbSet<Order> Orders { get; set; }
	public DbSet<ShipmentMethod> ShipmentMethods { get; set; }
	public DbSet<Customer> Customers { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<PaymentMethod> PaymentMethods { get; set; }

	// This method connects the context with the database
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "test.db" };
		var connectionString = connectionStringBuilder.ToString();
		var connection = new SqliteConnection(connectionString);

		optionsBuilder.UseSqlite(connection);
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
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
            this.Add(new Product { Name = "Блокнот", Cost = 230, ImageUrls = JsonConvert.SerializeObject(new string[] {"#"}), Description = "Desc", Characteristics = "Char" });
        }
		
		this.SaveChanges();

	}

}