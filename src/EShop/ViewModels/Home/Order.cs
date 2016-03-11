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

		// api properties
		public int Id { get; set; }
		public int AssigneeId { get; set; }
		public int OrderStatusId { get; set; }

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
					CustomerEmail: {(string.IsNullOrWhiteSpace(CustomerEmail) ? "no email" : CustomerEmail.Trim())}
					CustomerPhone: {CustomerPhone}
					PaymentMethodName:
						{PaymentMethodName}
					ShipmentMethodName: {ShipmentMethodName}
					Address:
						{(string.IsNullOrWhiteSpace(Address) ? "no address" : Address.Trim())}
					Comment:
						{(string.IsNullOrWhiteSpace(Comment) ? "no comment" : Comment.Trim())}
						
					TotalAmountDue:
						{TotalAmountDue} UAH
				";
		}
	}

    public class OrderIdViewModel
    {
        public int Id { get; set; }
    }
}