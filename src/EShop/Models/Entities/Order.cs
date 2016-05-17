using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models.Enitites
{

	[Table("Order")]
	public class Order
	{
		[Key]
		public int Id { get; set; }
		public int? AssigneeId { get; set; }
		public int OrderStatusId { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public int CustomerId { get; set; }
		public int ShipmentMethodId { get; set; }
		public string Address { get; set; }
		public int PaymentMethodId { get; set; }
		public string Comment { get; set; }
		public string AssigneeComment { get; set; }
		public long DateCreated { get; set; }
		public long DateLastModified { get; set; }

		// navigation properties		
		[ForeignKey("AssigneeId")]
		public User Assignee { get; set; }

		[ForeignKey("OrderStatusId")]
		public OrderStatus OrderStatus { get; set; }

		[ForeignKey("ProductId")]
		public Product Product { get; set; }

		[ForeignKey("CustomerId")]
		public Customer Customer { get; set; }

		[ForeignKey("ShipmentMethodId")]
		public ShipmentMethod ShipmentMethod { get; set; }

		[ForeignKey("PaymentMethodId")]
		public PaymentMethod PaymentMethod { get; set; }


		public override string ToString()
		{
			return $@"
				Order:
				
					{Product.ToString()}
					Quantity: {Quantity}
					{Customer.ToString()}
					{ShipmentMethod.ToString()}
					Address:
						{Address}
					{PaymentMethod.ToString()}
					Comment:
						{(string.IsNullOrWhiteSpace(Comment) ? "no comment" : Comment)}
					Date received:
						{new DateTime(DateCreated)}
				";
		}
	}

	[Table("ShipmentMethod")]
	public class ShipmentMethod
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public int Cost { get; set; }
		public bool CostTBD { get; set; }

		public override string ToString()
		{
			return $@"
				Shipment method:
				
					Name: {Name}
					Cost: {(CostTBD ? "to" : Cost.ToString())}
				";
		}
	}

	[Table("Customer")]
	public class Customer
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }

		public override string ToString()
		{
			return $@"
				Customer:
				
					Name: {Name}
					Phone: {Phone}
					Eamil: {Email}
				";
		}
	}

	[Table("Product")]
	public class Product
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public int Cost { get; set; }
		public string ImageUrls { get; set; }           // JSON list of URLs
		public string Description { get; set; }
		public string Characteristics { get; set; }

		public override string ToString()
		{
			return $@"
				Product:
				
					Name: {Name}
					Cost: {Cost}
				";
		}
	}

	[Table("PaymentMethod")]
	public class PaymentMethod
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }

		public override string ToString()
		{
			return $@"
				Payment method:
				
					Name: {Name}
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
	}

	[Table("OrderStatus")]
	public class OrderStatus
	{
		[Key]
		public int Id { get; set; }
		public string Description { get; set; }
	}

}