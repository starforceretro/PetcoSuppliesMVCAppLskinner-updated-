using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetcoSuppliesMVCAppLskinner.Data;
using PetcoSuppliesMVCAppLskinner.Models;

namespace PetcoSuppliesMVCAppLskinner.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // VIEW USER WISHLIST
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var wishlistItems = await _context.WishlistItems
                .Include(w => w.Product)
                .Where(w => w.UserId == userId)
                .ToListAsync();

            return View(wishlistItems);
        }

        // ADD PRODUCT TO WISHLIST
        public async Task<IActionResult> Add(int productId)
        {
            var userId = _userManager.GetUserId(User);

            var alreadyExists = await _context.WishlistItems
                .AnyAsync(w =>
                    w.ProductId == productId &&
                    w.UserId == userId);

            if (!alreadyExists)
            {
                var wishlistItem = new WishListItem
                {
                    ProductId = productId,
                    UserId = userId
                };

                _context.WishlistItems.Add(wishlistItem);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // REMOVE ITEM FROM WISHLIST
        public async Task<IActionResult> Remove(int id)
        {
            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.Id == id);

            if (item != null)
            {
                _context.WishlistItems.Remove(item);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}


