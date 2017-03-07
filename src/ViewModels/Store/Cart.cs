using System.Collections.Generic;
using System.Linq;
using Shevastream.Models.Entities;

namespace Shevastream.ViewModels.Store
{
	public class CartElementViewModel
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; }
	}
	
	public class CartViewModel
	{
		public ICollection<CartElementViewModel> Elements { get; set; }
	}
	
	public class FullCartElementViewModel
	{
		public Product Product { get; set; }
		public int Quantity { get; set; }
	}

	public class FullCartViewModel
	{
		public ICollection<FullCartElementViewModel> Products { get; set; }

		public int GetTotalCost() {
			return Products.Sum(prod => prod.Product.Cost * prod.Quantity);
		}

		public override string ToString()
		{
			return $@"
				Cart:
				
					{
						string.
							Join(
								"\n", 
								Products.
									Select(element => 
										string.Format(
											"{0} item(s) of {1}",
											element.Quantity, 
											element.Product.Name
										)
									)
								)
					}";
		}
	}
}