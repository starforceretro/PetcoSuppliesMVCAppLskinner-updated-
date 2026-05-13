using Microsoft.AspNetCore.Mvc;
using PetcoSuppliesMVCAppLskinner.Data;
using PetcoSuppliesMVCAppLskinner.Models;

namespace PetcoSuppliesMVCAppLskinner.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        // TEMP STATIC CART FOR TONIGHT
        private static List<CartItem> cart = new List<CartItem>();

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(cart);
        }

        public IActionResult AddToCart(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var existingItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1,
                    ImageUrl = product.ImageUrl
                });
            }

            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int id)
        {
            var item = cart.FirstOrDefault(c => c.ProductId == id);

            if (item != null)
            {
                cart.Remove(item);
            }

            return RedirectToAction("Index");
        }

        public IActionResult ClearCart()
        {
            cart.Clear();

            return RedirectToAction("Index");
        }
    }
}

