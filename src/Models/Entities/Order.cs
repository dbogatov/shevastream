using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shevastream.Models.Entities
{
	/// <summary>
	/// Represents a user made order
	/// </summary>
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

		/// <summary>
		/// Virtual property pointing to products order-product pairs
		/// </summary>
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

	/// <summary>
	/// Intermediate relation in a many-to-many relationship
	/// </summary>
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

	/// <summary>
	/// Represents a product we sell
	/// </summary>
	public class Product
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		/// <summary>
		/// Cost in the local currency units, eq. UAH
		/// </summary>
		public int Cost { get; set; }
		/// <summary>
		/// JSON list of URLs
		/// </summary>
		public string ImageUrls { get; set; }
		public string Information { get; set; }
		public string Description { get; set; }
		/// <summary>
		/// JSON list of properties
		/// May include HTML
		/// </summary>
		/// <returns></returns>
		public string Characteristics { get; set; }
		/// <summary>
		/// JSON object that has
		/// 	HasVideo: boolean - if this products has apromo video
		/// 	Url: string - URL for video if product has one 
		/// </summary>
		public string VideoData { get; set; }

		/// <summary>
		/// Virtual property pointing to products order-product pairs
		/// </summary>
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

	public class User
	{
		[Key]
		public int Id { get; set; }
		public string FullName { get; set; }
		public string NickName { get; set; }
		/// <summary>
		/// MD5 hash of the user password
		/// </summary>
		public string PassHash { get; set; }
		/// <summary>
		/// URL for avatar image
		/// </summary>
		public string ImageUrl { get; set; }
		public string Position { get; set; }
		public string Occupation { get; set; }
	}
}
