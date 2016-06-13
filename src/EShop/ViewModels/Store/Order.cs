using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EShop.ViewModels.Store
{
	public class OrderViewModel
	{
		[BindNever]
		[ScaffoldColumn(false)]
		public FullCartViewModel Cart { get; set; }

		[Required(ErrorMessage = "Поле \"Ім'я\" обов'язкове.")]
		[StringLength(80)]
		[Display(Name = "Ім'я *")]
		public string CustomerName { get; set; }

		[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Email is not valid.")]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "Email")]
		public string CustomerEmail { get; set; }

		[Required(ErrorMessage = "Поле \"Номер телефону\" обов'язкове.")]
		[StringLength(24)]
		[DataType(DataType.PhoneNumber)]
		[Display(Name = "Номер телефону *")]
		public string CustomerPhone { get; set; }

		[Required(ErrorMessage = "Поле \"Спосіб оплати\" обов'язкове.")]
		[Display(Name = "Спосіб оплати *")]
		public int PaymentMethodId { get; set; }

		[Required(ErrorMessage = "Поле \"Спосіб доставки\" обов'язкове.")]
		[Display(Name = "Спосіб доставки *")]
		public int ShipmentMethodId { get; set; }

		[Display(Name = "Адреса доставки *")]
		[MaxLength(5000)]
		public string Address { get; set; }

		[Display(Name = "Коментар")]
		[MaxLength(5000)]
		public string Comment { get; set; }

		// api properties

		[BindNever]
		[ScaffoldColumn(false)]
		public int Id { get; set; }

		[BindNever]
		[ScaffoldColumn(false)]
		public int AssigneeId { get; set; }

		[BindNever]
		[ScaffoldColumn(false)]
		public int OrderStatusId { get; set; }

		// display properties

		[BindNever]
		[ScaffoldColumn(false)]
		public string PaymentMethodName { get; set; }

		[BindNever]
		[ScaffoldColumn(false)]
		public string ShipmentMethodName { get; set; }

		[BindNever]
		[ScaffoldColumn(false)]
		public int TotalAmountDue { get; set; }


		public override string ToString()
		{
			return $@"
				Order Summary:
				
					Cart: {(Cart == null ? "cart not set" : Cart.ToString())}
					CustomerName: {CustomerName}
					CustomerEmail: {(string.IsNullOrWhiteSpace(CustomerEmail) ? "no email" : CustomerEmail.Trim())}
					CustomerPhone: {CustomerPhone}
					PaymentMethodName:
						{PaymentMethodName}
					ShipmentMethodName: 
						{ShipmentMethodName}
					Address:
						{(string.IsNullOrWhiteSpace(Address) ? "no address" : Address.Trim())}
					Comment:
						{(string.IsNullOrWhiteSpace(Comment) ? "no comment" : Comment.Trim())}
						
					TotalAmountDue:
						{TotalAmountDue} UAH
				";
		}
	}

	public class OrderTotalCostViewModel
	{
		public int TotalCost { get; set; }
	}

	public class OrderIdViewModel
	{
		public int Id { get; set; }
	}

	public class OrderStatusViewModel
	{
		public int Id { get; set; }
		public int Status { get; set; }
	}

	public class OrderUserData
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
		public string Phone { get; set; }
	}
}