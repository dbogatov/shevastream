using Shevastream.Models.Enitites;
using Shevastream.Services;
using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext
{
	public static string connectionString;
    public static string version;

    public DbSet<Feedback> Feedbacks { get; set; }

	public DbSet<LogEntry> LogEntries { get; set; }
	
	public DbSet<BlogPost> BlogPosts { get; set; }

    public DbSet<Order> Orders { get; set; }
	public DbSet<OrderProduct> OrderProducts { get; set; }
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
		//optionsBuilder.UseInMemoryDatabase();
		optionsBuilder.UseNpgsql(DataContext.connectionString);
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
        builder.HasDefaultSchema("shevastream");

        base.OnModelCreating(builder);
	}
}