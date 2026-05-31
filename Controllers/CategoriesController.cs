using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetcoSuppliesMVCAppLskinner.Data;
using PetcoSuppliesMVCAppLskinner.Models;

namespace PetcoSuppliesMVCAppLskinner.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context) // constructor with dependency injection
        {
            _context = context; // assign the injected database context to a private field for use in the controller's actions
        }

        // GET: Categories
        public async Task<IActionResult> Index() // an action method to retrieve and display a list of all categories from the database
        {
            return View(await _context.Categories.ToListAsync()); // use the database context to asynchronously retrieve all categories, convert them to a list, and pass that list to the view for rendering
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id) // an action method to retrieve and display the details of a specific category based on its ID
        {
            if (id == null)
            {
                return NotFound(); // if the ID parameter is null, return a 404 Not Found response to indicate that the requested resource cannot be found
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id); // use the database context to asynchronously retrieve the category with the specified ID, using a lambda expression to filter the categories based on their ID
            if (category == null)
            {
                return NotFound(); // if the category is not found (i.e., the query returns null), return a 404 Not Found response to indicate that the requested resource cannot be found
            }

            return View(category); // if the category is found, pass it to the view for rendering, allowing the user to see the details of the selected category
        }

        // GET: Categories/Create
        public IActionResult Create() // an action method to display a form for creating a new category, which simply returns the view associated with the Create action, allowing the user to input the details of the new category they wish to create
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)
        {
            if (ModelState.IsValid) // if the model state is valid (i.e., the data submitted by the user meets the validation requirements defined in the Category model), proceed to save the new category to the database
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category); // if the model state is not valid (i.e., there are validation errors), return the view with the current category model, allowing the user to correct any errors and resubmit the form
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id) // an action method to display a form for editing an existing category, which retrieves the category based on its ID and passes it to the view for editing
        {
            if (id == null)
            {
                return NotFound(); // if the ID parameter is null, return a 404 Not Found response to indicate that the requested resource cannot be found
            }

            var category = await _context.Categories.FindAsync(id); // use the database context to asynchronously find the category with the specified ID, which retrieves the category from the database based on its primary key (ID)
            if (category == null)
            {
                return NotFound();
            }
            return View(category); // if the category is found, pass it to the view for rendering, allowing the user to see the current details of the category and make any necessary edits
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category) // an action method to handle the submission of the edit form, which updates the existing category in the database with the new details provided by the user
        {
            if (id != category.Id) //   if the ID provided in the URL does not match the ID of the category being edited (i.e., the category's ID), return a 404 Not Found response to indicate that the requested resource cannot be found, as this could indicate a potential mismatch or tampering with the data
            {
                return NotFound();
            }

            if (ModelState.IsValid) // if the model state is valid (i.e., the data submitted by the user meets the validation requirements defined in the Category model), proceed to update the existing category in the database with the new details provided by the user
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync(); // use the database context to update the existing category in the database with the new details provided by the user, and save the changes asynchronously to persist the updates to the database
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id)) // if a concurrency exception occurs (i.e., another user has modified the same category in the database since it was retrieved for editing), check if the category still exists in the database using the CategoryExists method, which checks if a category with the specified ID exists in the database
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // if the update is successful, redirect to the Index action to display the list of categories, allowing the user to see the updated category in the list
            }
            return View(category); // if the model state is not valid (i.e., there are validation errors), return the view with the current category model, allowing the user to correct any errors and resubmit the form
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id) // an action method to display a confirmation page for deleting an existing category, which retrieves the category based on its ID and passes it to the view for confirmation before deletion
        {
            if (id == null)
            {
                return NotFound(); // if the ID parameter is null, return a 404 Not Found response to indicate that the requested resource cannot be found
            }

            var category = await _context.Categories // use the database context to asynchronously retrieve the category with the specified ID, using a lambda expression to filter the categories based on their ID, which retrieves the category from the database to confirm that it exists before displaying the confirmation page for deletion
                .FirstOrDefaultAsync(m => m.Id == id); // if the category is not found (i.e., the query returns null), return a 404 Not Found response to indicate that the requested resource cannot be found
            if (category == null)
            {
                return NotFound();
            }

            return View(category); // if the category is found, pass it to the view for rendering, allowing the user to see the details of the category they are about to delete and confirm their intention to delete it
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) // an action method to handle the confirmation of deletion, which deletes the existing category from the database based on its ID after the user confirms their intention to delete it
        {
            var category = await _context.Categories.FindAsync(id); // use the database context to asynchronously find the category with the specified ID, which retrieves the category from the database based on its primary key (ID) to confirm that it exists before attempting to delete it
            if (category != null)
            {
                _context.Categories.Remove(category); // if the category is found, remove it from the database context, which marks the category for deletion in the database
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); // after the category is deleted, save the changes to the database asynchronously to persist the deletion, and redirect to the Index action to display the updated list of categories, allowing the user to see that the category has been successfully deleted
        }

        private bool CategoryExists(int id) // a private helper method to check if a category with the specified ID exists in the database, which is used in the Edit action to handle concurrency exceptions and ensure that the category being edited still exists in the database before attempting to update it
        {
            return _context.Categories.Any(e => e.Id == id); //     use the database context to check if any category exists in the database with the specified ID, using a lambda expression to filter the categories based on their ID, and return true if such a category exists, or false if it does not exist
        }
    }
}
