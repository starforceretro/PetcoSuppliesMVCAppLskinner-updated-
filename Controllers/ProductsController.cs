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
        private readonly ApplicationDbContext _context; // database context for accessing the database
        private readonly IWebHostEnvironment _webHostEnvironment; // environment for accessing web root path for saving uploaded images

        public ProductsController( // constructor with dependency injection
      ApplicationDbContext context,
      IWebHostEnvironment webHostEnvironment)
        {
            _context = context; // assign the injected database context to the private field
            _webHostEnvironment = webHostEnvironment; // assign the injected environment to the private field
        }

        [Authorize(Roles = "Admin,Staff")] // only allow admin and staff to access the edit page
        public async Task<IActionResult> Edit(int? id) // action method to display the edit form for a product, requires the user to be in the Admin or Staff role, accepts an optional product ID as a parameter, retrieves the product from the database, and if it exists, passes it to the view for editing
        {
            if (id == null) // if no ID is provided, return a 404 Not Found response
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id); // find the product by ID in the database, if the product is not found, return a 404 Not Found response

            if (product == null) // if the product with the specified ID does not exist in the database, return a 404 Not Found response
            {
                return NotFound();
            }

            ViewData["CategoryId"] = // send categories to view for dropdown selection, creating a SelectList from the Categories DbSet, specifying the value field as "Id", the text field as "Name", and setting the selected value to the CategoryId of the product being edited
                new SelectList(_context.Categories,
                "Id",
                "Name",
                product.CategoryId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")] // only allow admin and staff to submit the edit form, accepts the product ID and the updated product object as parameters, checks if the ID matches the product's ID, handles image upload if a new image is provided, updates the product in the database, and redirects to the index page if successful
        public async Task<IActionResult> Edit(int id, Product product) // action method to handle the form submission for editing a product, requires the user to be in the Admin or Staff role, accepts the product ID and the updated product object as parameters, checks if the ID matches the product's ID, handles image upload if a new image is provided, updates the product in the database, and redirects to the index page if successful
        {
            if (id != product.Id)
            {
                return NotFound(); // if the provided ID does not match the ID of the product being edited, return a 404 Not Found response
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // get existing product from database to access current image URL if needed

                    var existingProduct = // retrieve the existing product from the database using AsNoTracking to avoid tracking changes to it, which allows us to access the current image URL without affecting the state of the entity in the context
                        await _context.Products.AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (existingProduct == null) // if the product with the specified ID does not exist in the database, return a 404 Not Found response
                    {
                        return NotFound();
                    }

                    // keep existing image URL if no new image is uploaded

                    product.ImageUrl = existingProduct.ImageUrl;

                    // new image upload

                    if (product.ImageFile != null) // if a new image file is uploaded, handle the file upload process, which includes saving the new image to the server and updating the ImageUrl property of the product with the path to the new image
                    {
                        string uploadsFolder = Path.Combine( // determine the folder path for saving uploaded product images by combining the web root path with a specific folder for product images
                            _webHostEnvironment.WebRootPath,
                            "images/products");

                        Directory.CreateDirectory(uploadsFolder); // create the directory if it doesn't exist to ensure that the application can save the uploaded images without errors

                        string fileExtension =
                            Path.GetExtension(product.ImageFile.FileName); // get the file extension of the uploaded image file to preserve the original file type when saving the new image

                        string uniqueFileName =
                            Guid.NewGuid().ToString() + fileExtension;

                        string filePath =
                            Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream =
                               new FileStream(filePath, FileMode.Create))
                        {
                            await product.ImageFile.CopyToAsync(fileStream); // save the new image file to the server using a file stream, which allows for efficient writing of the uploaded file to the specified location on the server
                        }

                        product.ImageUrl =
                            "/images/products/" + uniqueFileName; // update the ImageUrl property of the product with the relative URL of the new image, which can be used to display the new image in the product details and list views
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

                return RedirectToAction(nameof(Index)); // if the update is successful, redirect to the index page to show the updated list of products
            }

            ViewData["CategoryId"] =
                new SelectList(
                    _context.Categories,
                    "Id",
                    "Name",
                    product.CategoryId);

            return View(product);
        }

        [Authorize(Roles = "Admin,Staff")] // only allow admin and staff to access the delete page, accepts an optional product ID as a parameter, retrieves the product from the database, and if it exists, passes it to the view for confirmation before deletion
        public async Task<IActionResult> Delete(int? id) // action method to display the delete confirmation page for a product, requires the user to be in the Admin or Staff role, accepts an optional product ID as a parameter, retrieves the product from the database, and if it exists, passes it to the view for confirmation before deletion
        {
            if (id == null) // if no ID is provided, return a 404 Not Found response
            {
                return NotFound(); // if no ID is provided, return a 404 Not Found response
            }

            var product = await _context.Products // retrieve the product from the database using the provided ID, including the related Category entity to display category information on the delete confirmation page
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) // if the product with the specified ID does not exist in the database, return a 404 Not Found response
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")] // action method to handle the form submission for confirming the deletion of a product, requires the user to be in the Admin or Staff role, accepts the product ID as a parameter, retrieves the product from the database, and if it exists, removes it from the database and redirects to the index page
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")] // only allow admin and staff to submit the delete confirmation form, accepts the product ID as a parameter, retrieves the product from the database, and if it exists, removes it from the database and redirects to the index page
        public async Task<IActionResult> DeleteConfirmed(int id) // action method to handle the form submission for confirming the deletion of a product, requires the user to be in the Admin or Staff role, accepts the product ID as a parameter, retrieves the product from the database, and if it exists, removes it from the database and redirects to the index page
        {
            var product = await _context.Products.FindAsync(id); // retrieve the product from the database using the provided ID, if the product is not found, it will be null

            if (product != null) // if the product with the specified ID exists in the database, remove it from the Products DbSet and save the changes to the database to reflect the deletion of the product
            {
                _context.Products.Remove(product);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchString, int? categoryId) // action method to display the list of products, accepts optional search string and category ID parameters for filtering the products, retrieves the products from the database including their related Category entities, applies the search and category filters if provided, sends the list of categories to the view for dropdown selection, and returns the view with the filtered list of products
        {
            var products = _context.Products // retrieve the products from the database, including their related Category entities to display category information in the product list view, and convert the result to an IQueryable for further filtering based on the search string and category ID parameters
       .Include(p => p.Category)
       .AsQueryable();

            // SEARCH

            if (!string.IsNullOrEmpty(searchString)) // if a search string is provided, filter the products to include only those whose names contain the search string, allowing for partial matches and case-insensitive searching
            {
                products = products.Where(p =>
                    p.Name.Contains(searchString));
            }

            // CATEGORY FILTER

            if (categoryId.HasValue) // if a category ID is provided, filter the products to include only those that belong to the specified category, allowing users to narrow down the product list based on their category selection
            {
                products = products.Where(p =>
                    p.CategoryId == categoryId);
            }

            // SEND CATEGORIES TO VIEW

            ViewBag.Categories = _context.Categories.ToList();

            return View(await products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id) // action method to display the details of a specific product, accepts an optional product ID as a parameter, retrieves the product from the database including its related Category entity, and if the product exists, passes it to the view for display
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products // retrieve the product from the database using the provided ID, including the related Category entity to display category information on the product details page, and if the product with the specified ID does not exist in the database, return a 404 Not Found response
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [Authorize(Roles = "Admin,Staff")] //   only allow admin and staff to access the create page, action method to display the form for creating a new product, requires the user to be in the Admin or Staff role, sends the list of categories to the view for dropdown selection, and returns the view for creating a new product
        public IActionResult Create() //    action method to display the form for creating a new product, requires the user to be in the Admin or Staff role, sends the list of categories to the view for dropdown selection, and returns the view for creating a new product
        {
            ViewData["CategoryId"] =
                new SelectList(_context.Categories, "Id", "Name"); // send the list of categories to the view for dropdown selection, creating a SelectList from the Categories DbSet, specifying the value field as "Id" and the text field as "Name"

            return View();
        }

        // GET: Products/Create
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")] // only allow admin and staff to submit the create form, accepts the new product object as a parameter, checks if the model state is valid, handles image upload if an image file is provided, adds the new product to the database, and redirects to the index page if successful
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product) // action method to handle the form submission for creating a new product, requires the user to be in the Admin or Staff role, accepts the new product object as a parameter, checks if the model state is valid, handles image upload if an image file is provided, adds the new product to the database, and redirects to the index page if successful
        {
            if (ModelState.IsValid) // if the model state is valid, proceed with creating the new product, which includes handling the image upload if an image file is provided, adding the new product to the database context, saving the changes to the database, and redirecting to the index page to display the updated list of products
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
