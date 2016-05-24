using System.Collections.Generic;
using EShop.Models.Enitites;

namespace EShop.ViewModels.Store
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
    }
}