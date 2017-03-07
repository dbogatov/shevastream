using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EShop.Services;

namespace EShop.Views.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        readonly ICartService _cart;

        public CartViewComponent(ICartService cart)
		{
            _cart = cart;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var count = _cart.GetCart().Products.Count;

            // hack
            await Task.FromResult(0);
            return View((object)(count > 0 ? $" ({count})" : "" ));
        }
    }
}