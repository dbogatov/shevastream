using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.ViewModels.Home
{
	public class OrderViewModel
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public string CustomerName { get; set; }
		public string CustomerEmail { get; set; }
		public string CustomerPhone { get; set; }
		public int PaymentMethodId { get; set; }
		public int ShipmentMethodId { get; set; }
		public string Address { get; set; }
		public string Comment { get; set; }

		// display properties
		public string ProductName { get; set; }
		public string PaymentMethodName { get; set; }
		public string ShipmentMethodName { get; set; }
		public int TotalAmountDue { get; set; }


		public override string ToString()
		{	
			return $@"
				Order Summary:
				
					ProductName:{ProductName}
					Quantity: {Quantity}
					CustomerName: {CustomerName}
					CustomerEmail: {CustomerEmail}
					CustomerPhone: {CustomerPhone}
					PaymentMethodName:
						{PaymentMethodName}
					ShipmentMethodName: {ShipmentMethodName}
					Address:
						{Address}
					Comment:
						{(string.IsNullOrWhiteSpace(Comment) ? "no comment" : Comment.Trim())}
						
					TotalAmountDue:
						{TotalAmountDue} UAH
				";
		}
	}
}