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
        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment _environment;

        private readonly UserManager<ApplicationUser> _userManager;

        public PetGalleryController(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        // VIEW GALLERY
        public async Task<IActionResult> Index()
        {
            var posts = await _context.PetGalleryPosts
                .OrderByDescending(p => p.UploadDate)
                .ToListAsync();

            return View(posts);
        }

        // CREATE PAGE
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE POST
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(
            PetGalleryPost post,
            IFormFile imageFile)
        {
            if (imageFile != null)
            {
                // CREATE UNIQUE FILE NAME
                var fileName = Guid.NewGuid().ToString()
                    + Path.GetExtension(imageFile.FileName);

                // FILE PATH
                var uploadPath = Path.Combine(
                    _environment.WebRootPath,
                    "images/pets");

                // CREATE DIRECTORY IF IT DOES NOT EXIST
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                // SAVE FILE
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // SAVE URL TO DATABASE
                post.ImageUrl = "/images/pets/" + fileName;
            }

            post.UploadDate = DateTime.Now;

            post.UserId = _userManager.GetUserId(User);

            _context.PetGalleryPosts.Add(post);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // DELETE POST
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.PetGalleryPosts
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post != null)
            {
                _context.PetGalleryPosts.Remove(post);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
