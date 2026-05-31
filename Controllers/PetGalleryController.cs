using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetcoSuppliesMVCAppLskinner.Data;
using PetcoSuppliesMVCAppLskinner.Models;

namespace PetcoSuppliesMVCAppLskinner.Controllers
{
    public class PetGalleryController : Controller
    {
        private readonly ApplicationDbContext _context; // database context for accessing the database

        private readonly IWebHostEnvironment _environment; // environment for accessing web root path for saving uploaded images

        private readonly UserManager<ApplicationUser> _userManager; // user manager for accessing user information, such as the current user's ID

        public PetGalleryController( // constructor with dependency injection
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            UserManager<ApplicationUser> userManager)
        {
            _context = context; // assign the injected database context to the private field
            _environment = environment; // assign the injected environment to the private field
            _userManager = userManager; //  assign the injected user manager to the private field
        }

        // view gallery of pet photos
        public async Task<IActionResult> Index() // action method to display the pet gallery, retrieves all pet gallery posts from the database, ordered by upload date, and passes them to the view
        {
            var posts = await _context.PetGalleryPosts // get all pet gallery posts from the database
                .OrderByDescending(p => p.UploadDate)
                .ToListAsync();

            return View(posts);
        }

        // create page for uploading pet photos
        [Authorize]
        public IActionResult Create() // action method to display the form for creating a new pet gallery post, requires the user to be authenticated, simply returns the view for the create form
        {
            return View();
        }

        // CREATE POST
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create( // action method to handle the form submission for creating a new pet gallery post, requires the user to be authenticated, accepts a PetGalleryPost model and an uploaded image file as parameters
            PetGalleryPost post,
            IFormFile imageFile)
        {
            if (imageFile != null)
            {
                // create unique file name using a GUID and the original file extension to avoid conflicts with existing files
                var fileName = Guid.NewGuid().ToString()
                    + Path.GetExtension(imageFile.FileName);

                // file path to save the uploaded image, combining the web root path with a specific folder for pet images
                var uploadPath = Path.Combine(
                    _environment.WebRootPath,
                    "images/pets");

                // create the directory if it doesn't exist to ensure that the application can save the uploaded images without errors
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                // save file to server using a file stream, which allows for efficient writing of the uploaded file to the specified location on the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // save the relative URL of the uploaded image to the database, which can be used to display the image in the pet gallery
                post.ImageUrl = "/images/pets/" + fileName;
            }

            post.UploadDate = DateTime.Now; // set the upload date to the current date and time when the post is created

            post.UserId = _userManager.GetUserId(User); // set the UserId property of the pet gallery post to the ID of the currently logged-in user, which allows for tracking which user uploaded each pet photo

            _context.PetGalleryPosts.Add(post);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // after successfully creating a new pet gallery post, redirect the user to the Index action to display the updated gallery with the new post included
        }

        // delete post
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int id) // action method to handle the deletion of a pet gallery post, requires the user to be authenticated, accepts the ID of the post to be deleted as a parameter, retrieves the post from the database, and if it exists, removes it from the database and saves the changes before redirecting back to the Index action to display the updated gallery without the deleted post
        {
            var post = await _context.PetGalleryPosts // find the pet gallery post by its ID, which is passed as a parameter to the action method
                .FirstOrDefaultAsync(p => p.Id == id); // if a post with the specified ID is found in the database, it is removed from the PetGalleryPosts DbSet, and the changes are saved to the database to reflect the deletion of the post

            if (post != null) // if the post exists, remove it from the database and save changes
            {
                _context.PetGalleryPosts.Remove(post); // remove the post from the database context, which marks it for deletion when SaveChangesAsync is called

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index"); // after attempting to delete the post (regardless of whether it was found and deleted or not), redirect the user back to the Index action to display the updated gallery, which will no longer include the deleted post if it was successfully removed
        }

        [Authorize(Roles = "Admin,Staff")] // action method to display the moderation page for pet gallery posts, requires the user to be authenticated and in either the "Admin" or "Staff" role, retrieves all pet gallery posts from the database, including the associated user information, ordered by upload date, and passes them to the view for moderation purposes
        public async Task<IActionResult> Moderate() // action method to display the moderation page for pet gallery posts, requires the user to be authenticated and in either the "Admin" or "Staff" role, retrieves all pet gallery posts from the database, including the associated user information, ordered by upload date, and passes them to the view for moderation purposes
        {
            var posts = await _context.PetGalleryPosts // get all pet gallery posts from the database, including the associated user information, ordered by upload date
                .Include(p => p.User)
                .OrderByDescending(p => p.UploadDate)
                .ToListAsync();

            return View(posts); // return the view associated with this action (Views/PetGallery/Moderate.cshtml) and pass the list of pet gallery posts to the view for display and moderation purposes
        }
    }
}
