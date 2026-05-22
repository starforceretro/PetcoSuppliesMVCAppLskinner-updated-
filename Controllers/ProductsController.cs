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

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] =
                new SelectList(_context.Categories,
                "Id",
                "Name",
                product.CategoryId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // GET EXISTING PRODUCT FROM DATABASE

                    var existingProduct =
                        await _context.Products.AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // KEEP OLD IMAGE BY DEFAULT

                    product.ImageUrl = existingProduct.ImageUrl;

                    // NEW IMAGE UPLOAD

                    if (product.ImageFile != null)
                    {
                        string uploadsFolder = Path.Combine(
                            _webHostEnvironment.WebRootPath,
                            "images/products");

                        Directory.CreateDirectory(uploadsFolder);

                        string fileExtension =
                            Path.GetExtension(product.ImageFile.FileName);

                        string uniqueFileName =
                            Guid.NewGuid().ToString() + fileExtension;

                        string filePath =
                            Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream =
                               new FileStream(filePath, FileMode.Create))
                        {
                            await product.ImageFile.CopyToAsync(fileStream);
                        }

                        product.ImageUrl =
                            "/images/products/" + uniqueFileName;
                    }

                    _context.Update(product);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.Id == product.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

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

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int? id)
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                _context.Products.Remove(product);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
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
