using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetcoSuppliesMVCAppLskinner.Data;
using PetcoSuppliesMVCAppLskinner.Models;

namespace PetcoSuppliesMVCAppLskinner.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }




        // VIEW CART

        public async Task<IActionResult> Index()
        {
            var user =
                await _userManager.GetUserAsync(User);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            // CREATE EMPTY CART IF USER HAS NONE

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = user.Id,
                    CartItems = new List<CartItem>()
                };

                _context.Carts.Add(cart);

                await _context.SaveChangesAsync();
            }

            return View(cart);
        }

        // ADD PRODUCT TO CART

        public async Task<IActionResult> AddToCart(int id)
        {
            var user =
                await _userManager.GetUserAsync(User);

            var product =
                await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // FIND USER CART

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            // CREATE CART IF NONE EXISTS

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = user.Id,
                    CartItems = new List<CartItem>()
                };

                _context.Carts.Add(cart);

                await _context.SaveChangesAsync();
            }

            // CHECK IF PRODUCT ALREADY EXISTS

            var existingItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == id);

            if (existingItem != null)
            {
                // PREVENT GOING ABOVE STOCK

                if (existingItem.Quantity >= product.Stock)
                {
                    TempData["Error"] =
                        "No more stock available.";

                    return RedirectToAction(
                        "Index",
                        "Products");
                }

                existingItem.Quantity++;
            }
            else
            {
                // STOP IF PRODUCT OUT OF STOCK

                if (product.Stock <= 0)
                {
                    TempData["Error"] =
                        "Product is out of stock.";

                    return RedirectToAction(
                        "Index",
                        "Products");
                }

                var cartItem = new CartItem
                {
                    ProductId = product.Id,
                    Quantity = 1,
                    CartId = cart.Id
                };

                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Product added to cart.";

            return RedirectToAction(
                "Index",
                "Products");
        }

        // REMOVE ENTIRE ITEM FROM CART

        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var user =
                await _userManager.GetUserAsync(User);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var item = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == id);

            if (item != null)
            {
                _context.CartItems.Remove(item);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // INCREASE QUANTITY

        public async Task<IActionResult> IncreaseQuantity(int id)
        {
            var user =
                await _userManager.GetUserAsync(User);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var item = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == id);

            if (item != null)
            {
                if (item.Quantity < item.Product.Stock)
                {
                    item.Quantity++;

                    await _context.SaveChangesAsync();
                }
                else
                {
                    TempData["Error"] =
                        "Cannot exceed available stock.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // DECREASE QUANTITY

        public async Task<IActionResult> DecreaseQuantity(int id)
        {
            var user =
                await _userManager.GetUserAsync(User);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var item = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == id);

            if (item != null)
            {
                item.Quantity--;

                // REMOVE ITEM IF QUANTITY HITS 0

                if (item.Quantity <= 0)
                {
                    _context.CartItems.Remove(item);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // CLEAR ENTIRE CART

        public async Task<IActionResult> ClearCart()
        {
            var user =
                await _userManager.GetUserAsync(User);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}