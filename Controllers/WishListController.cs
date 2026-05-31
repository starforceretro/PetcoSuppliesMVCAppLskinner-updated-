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
        private readonly ApplicationDbContext _context; // database context for accessing the database

        private readonly UserManager<ApplicationUser> _userManager; // user manager for accessing user information, such as the current user's ID

        public WishlistController( // constructor with dependency injection
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context; // assign the injected database context to the private field
            _userManager = userManager;
        }

        // view wishlist
        public async Task<IActionResult> Index() // action to display the user's wishlist
        {
            var userId = _userManager.GetUserId(User); // get the current user's ID using the user manager, which is necessary to retrieve the wishlist items that belong to the logged-in user

            var wishlistItems = await _context.WishlistItems
                .Include(w => w.Product)
                .Where(w => w.UserId == userId)
                .ToListAsync();

            return View(wishlistItems); // pass the list of wishlist items to the view (Views/Wishlist/Index.cshtml) to display the user's wishlist
        }

        // add item to wishlist
        public async Task<IActionResult> Add(int productId) // action to add a product to the user's wishlist, accepts the product ID as a parameter, checks if the item already exists in the wishlist to prevent duplicates, and if it doesn't exist, creates a new wishlist item and saves it to the database before redirecting back to the Index action to display the updated wishlist
        {
            var userId = _userManager.GetUserId(User); // get the current user's ID using the user manager, which is necessary to associate the new wishlist item with the logged-in user in the database

            var alreadyExists = await _context.WishlistItems // check if the product is already in the user's wishlist to prevent duplicate entries, which helps maintain data integrity and provides a better user experience by avoiding multiple entries of the same product in the wishlist
                .AnyAsync(w =>
                    w.ProductId == productId &&
                    w.UserId == userId);

            if (!alreadyExists) // if the product is not already in the wishlist, create a new wishlist item and save it to the database, which allows the user to add products to their wishlist and view them later
            {
                var wishlistItem = new WishListItem // create a new wishlist item with the specified product ID and the current user's ID, which will be saved to the database to represent the association between the user and the product in their wishlist
                {
                    ProductId = productId,
                    UserId = userId
                };

                _context.WishlistItems.Add(wishlistItem);

                await _context.SaveChangesAsync(); // save the new wishlist item to the database
            }

            return RedirectToAction("Index"); // after attempting to add the product to the wishlist (whether it was added or already existed), redirect the user back to the Index action to display the updated wishlist, allowing them to see the changes they made or confirm that the item was already in their wishlist
        }

        // remove item from wishlist
        public async Task<IActionResult> Remove(int id) // action to remove an item from the user's wishlist, accepts the ID of the wishlist item to be removed as a parameter, retrieves the item from the database, and if it exists, removes it from the database before redirecting back to the Index action to display the updated wishlist without the removed item
        {
            var item = await _context.WishlistItems // find the wishlist item by its ID, which is passed as a parameter to the action method
                .FirstOrDefaultAsync(w => w.Id == id); // if a wishlist item with the specified ID is found in the database, it is removed from the WishlistItems DbSet, and the changes are saved to the database to reflect the deletion of the item from the user's wishlist

            if (item != null) // if the item exists, remove it from the database and save changes
            {
                _context.WishlistItems.Remove(item); // remove the item from the database context, which marks it for deletion when SaveChangesAsync is called

                await _context.SaveChangesAsync(); // save the changes to the database to reflect the removal of the item from the user's wishlist
            }

            return RedirectToAction("Index"); // after attempting to remove the item from the wishlist (whether it was removed or not found), redirect the user back to the Index action to display the updated wishlist, allowing them to see the changes they made or confirm that the item was already removed from their wishlist
        }
    }
}


