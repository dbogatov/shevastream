using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shevastream.Models.Entities
{
	public class Order
	{
		[Key]
		public int Id { get; set; }
		public string CustomerName { get; set; }
		public string CustomerPhone { get; set; }
		public string CustomerEmail { get; set; }
		public string ShipmentMethod { get; set; }
		public string Address { get; set; }
		public string PaymentMethod { get; set; }
		public string Comment { get; set; }
		public DateTime DateCreated { get; set; } = DateTime.Now;

		public ICollection<OrderProduct> OrderProducts { get; set; }

		public override string ToString()
		{
			return $@"
				Order:
					{CustomerName} | {CustomerPhone} | {CustomerEmail}
					{ShipmentMethod}
					Address:
						{Address}
					{PaymentMethod}
					Comment:
						{(string.IsNullOrWhiteSpace(Comment) ? "no comment" : Comment)}
					Date received:
						{DateCreated}
				";
		}
	}

	[Table("OrderProduct")]
	public class OrderProduct
	{
		[Key]
		public int Id { get; set; }

		public int OrderId { get; set; }
		[ForeignKey("OrderId")]
		public Order Order { get; set; }

		public int ProductId { get; set; }
		[ForeignKey("ProductId")]
		public Product Product { get; set; }

		public int Quantity { get; set; }
	}

	public class Product
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public int Cost { get; set; }
		public string ImageUrls { get; set; }           // JSON list of URLs
		public string Information { get; set; }
		public string Description { get; set; }
		public string Characteristics { get; set; }
		public string VideoData { get; set; }

		public ICollection<OrderProduct> OrderProducts { get; set; }

		public override string ToString()
		{
			return $@"
				Product:
				
					Name: {Name}
					Cost: {Cost}
				";
		}
	}

	[Table("User")]
	public class User
	{
		[Key]
		public int Id { get; set; }
		public string FullName { get; set; }
		public string NickName { get; set; }
		public string PassHash { get; set; }
		public string ImageUrl { get; set; }
		public string Position { get; set; }
		public string Occupation { get; set; }
	}
}