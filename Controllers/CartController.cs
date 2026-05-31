using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetcoSuppliesMVCAppLskinner.Data;
using PetcoSuppliesMVCAppLskinner.Models;

namespace PetcoSuppliesMVCAppLskinner.Controllers
{
    [Authorize] // all cart actions require the user to be logged in
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context; // database context for accessing carts, products, and cart items

        private readonly UserManager<ApplicationUser> _userManager; // user manager to get current user information

        public CartController( // constructor to inject dependencies
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }




        // view cart

        public async Task<IActionResult> Index()
        {
            var user =
                await _userManager.GetUserAsync(User); // get the currently logged in user

            var cart = await _context.Carts // find the cart for the current user, including the cart items and their associated products
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            // create cart if none exists

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = user.Id, // associate the new cart with the current user
                    CartItems = new List<CartItem>() // initialize an empty list of cart items
                };

                _context.Carts.Add(cart); // add the new cart to the database context

                await _context.SaveChangesAsync();
            }

            return View(cart);
        }

        // add product to cart

        public async Task<IActionResult> AddToCart(int id) // id is the product id
        {
            var user =
                await _userManager.GetUserAsync(User); // get the currently logged in user

            var product =
                await _context.Products.FindAsync(id); // find the product by id to check stock and get product details

            if (product == null)
            {
                return NotFound(); // if the product is not found, return a 404 Not Found response
            }

            // find user's cart, including items

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == user.Id); // find the cart for the current user, including the cart items

            // create cart if none exists

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = user.Id,
                    CartItems = new List<CartItem>() // initialize an empty list of cart items for the new cart
                };

                _context.Carts.Add(cart); // add the new cart to the database context

                await _context.SaveChangesAsync(); // save changes to generate cart ID for new cart
            }

            // check if product already in cart

            var existingItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == id);

            if (existingItem != null)
            {
                // prevent increasing quantity beyond available stock

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
                // stop adding to cart if product is out of stock

                if (product.Stock <= 0)
                {
                    TempData["Error"] =
                        "Product is out of stock.";

                    return RedirectToAction( // redirect back to products page with error message
                        "Index",
                        "Products");
                }

                var cartItem = new CartItem // create a new cart item for the product being added to the cart
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

        // remove product from cart

        public async Task<IActionResult> RemoveFromCart(int id) // id is the product id
        {
            var user =
                await _userManager.GetUserAsync(User); // get the currently logged in user

            var cart = await _context.Carts // find the cart for the current user, including the cart items
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                return RedirectToAction(nameof(Index)); // if the cart is not found, redirect to the cart index page (which will create a new cart if needed)
            }

            var item = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == id); // find the cart item for the specified product id

            if (item != null)
            {
                _context.CartItems.Remove(item); // remove the cart item from the database context

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        //increase quantity

        public async Task<IActionResult> IncreaseQuantity(int id) // id is the product id
        {
            var user =
                await _userManager.GetUserAsync(User); // get the currently logged in user

            var cart = await _context.Carts // find the cart for the current user, including the cart items and their associated products (to check stock)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == user.Id); // find the cart for the current user, including the cart items and their associated products

            if (cart == null)
            {
                return RedirectToAction(nameof(Index)); // if the cart is not found, redirect to the cart index page (which will create a new cart if needed)
            }

            var item = cart.CartItems // find the cart item for the specified product id
                .FirstOrDefault(ci => ci.ProductId == id);

            if (item != null) // if the cart item is found, check if increasing the quantity would exceed available stock
            {
                if (item.Quantity < item.Product.Stock) // check if the current quantity is less than the available stock for the product
                {
                    item.Quantity++;

                    await _context.SaveChangesAsync(); // save changes to the database context to update the cart item quantity
                }
                else
                {
                    TempData["Error"] =
                        "Cannot exceed available stock."; // if increasing the quantity would exceed available stock, set an error message in TempData to display to the user
                }
            }

            return RedirectToAction(nameof(Index)); // redirect back to the cart index page to show the updated cart
        }

        // decrease quantity

        public async Task<IActionResult> DecreaseQuantity(int id) // id is the product id
        {
            var user =
                await _userManager.GetUserAsync(User); // get the currently logged in user

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == user.Id); // find the cart for the current user, including the cart items

            if (cart == null)
            {
                return RedirectToAction(nameof(Index)); // if the cart is not found, redirect to the cart index page (which will create a new cart if needed)
            }

            var item = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == id); // find the cart item for the specified product id

            if (item != null) // if the cart item is found, decrease the quantity by 1
            {
                item.Quantity--;

                // remove item if quantity is zero or less

                if (item.Quantity <= 0)
                {
                    _context.CartItems.Remove(item); // if the quantity is zero or less after decreasing, remove the cart item from the database context
                }

                await _context.SaveChangesAsync(); // save changes to the database context to update the cart item quantity or remove it if necessary
            }

            return RedirectToAction(nameof(Index));
        }

        // clear entire cart

        public async Task<IActionResult> ClearCart() // this action will remove all items from the user's cart
        {
            var user =
                await _userManager.GetUserAsync(User); // get the currently logged in user

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == user.Id); // find the cart for the current user, including the cart items

            if (cart != null) // if the cart is found, remove all cart items from the database context
            {
                _context.CartItems.RemoveRange(cart.CartItems); // remove all cart items from the database context using RemoveRange for efficiency

                await _context.SaveChangesAsync(); // save changes to the database context to clear the cart
            }

            return RedirectToAction(nameof(Index)); // redirect back to the cart index page to show the now empty cart
        }
    }
}