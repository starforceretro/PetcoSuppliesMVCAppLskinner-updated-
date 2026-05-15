using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetcoSuppliesMVCAppLskinner.Data;
using PetcoSuppliesMVCAppLskinner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetcoSuppliesMVCAppLskinner.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(
      ApplicationDbContext context,
      IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchString, int? categoryId)
        {
            var products = _context.Products
       .Include(p => p.Category)
       .AsQueryable();

            // SEARCH

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p =>
                    p.Name.Contains(searchString));
            }

            // CATEGORY FILTER

            if (categoryId.HasValue)
            {
                products = products.Where(p =>
                    p.CategoryId == categoryId);
            }

            // SEND CATEGORIES TO VIEW

            ViewBag.Categories = _context.Categories.ToList();

            return View(await products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] =
                new SelectList(_context.Categories, "Id", "Name");

            return View();
        }

        // GET: Products/Create
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                // IMAGE UPLOAD

                if (product.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "images/products");

                    // creates folder automatically if missing

                    Directory.CreateDirectory(uploadsFolder);

                    // unique filename

                    string fileExtension = Path.GetExtension(product.ImageFile.FileName);

                    string uniqueFileName =
                        Guid.NewGuid().ToString() + fileExtension;

                    string filePath =
                        Path.Combine(uploadsFolder, uniqueFileName);

                    // save image file

                    using (var fileStream =
                           new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }

                    // save path to database

                    product.ImageUrl =
                        "/images/products/" + uniqueFileName;
                }

                _context.Add(product);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] =
                new SelectList(
                    _context.Categories,
                    "Id",
                    "Name",
                    product.CategoryId);

            return View(product);
        }
    }
}
