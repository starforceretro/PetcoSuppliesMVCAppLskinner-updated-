using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetcoSuppliesMVCAppLskinner.Data;
using PetcoSuppliesMVCAppLskinner.Models;
using PetcoSuppliesMVCAppLskinner.Models.ViewModels;

namespace PetcoSupplies.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminController( // constructor with dependency injection
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;

            _userManager = userManager;

            _signInManager = signInManager;
        }

        public IActionResult Index() // a page to list all users with options to view details, edit, or delete
        {
            var users = _context.Users.ToList(); // get all users from the database

            return View(users); // pass the list of users to the view
        }


        public IActionResult CreateUser() // a page to create a new user, including their details and role assignment
        {
            return View();
        }

        public async Task<IActionResult> Details(string id) // a page to show the details of a user, including their orders
        {
            var user = await _context.Users // get the user from the database, including their orders
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        public async Task<IActionResult> EditUser(string id) // a page to edit the details of a user, including their orders
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) // if the user is not found, return a 404 Not Found response
                return NotFound();


            if (user == null)
                return NotFound();


            var model = new AdminUserViewModel // create a view model to pass to the view
            {
                Id = user.Id, // populate the view model with the user's current details
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Street = user.Street,
                Town = user.Town,
                PostCode = user.PostCode,
                TelephoneNumber = user.TelephoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(AdminUserViewModel model)
        {
            if (!ModelState.IsValid) // if the model state is not valid, return the view with the current model to show validation errors
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id); // find the user in the database by their ID

            if (user == null) // if the user is not found, return a 404 Not Found response
                return NotFound();

            // Update user details

            user.FirstName = model.FirstName; // update the user's first name with the value from the form
            user.LastName = model.LastName;
            user.Street = model.Street;
            user.Town = model.Town;
            user.PostCode = model.PostCode;
            user.TelephoneNumber = model.TelephoneNumber;

            user.Email = model.Email;
            user.UserName = model.Email;

            // OPTIONAL password reset

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var token =
                    await _userManager.GeneratePasswordResetTokenAsync(user); // generate a password reset token for the user

                var passwordResult =
                    await _userManager.ResetPasswordAsync(
                        user,
                        token,
                        model.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }
            }

            // Save changes to the database 

            var result = await _userManager.UpdateAsync(user); // update the user in the database

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index)); // if the update is successful, redirect to the index page to show the list of users
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description); // if there are errors during the update, add them to the model state to show validation errors in the view
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(AdminUserViewModel model) // handle the form submission for creating a new user
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email, // use email as username
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Street = model.Street,
                    Town = model.Town,
                    PostCode = model.PostCode,
                    TelephoneNumber = model.TelephoneNumber,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync( // create the user in the database with the specified password
                    user,
                    model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role); // assign the specified role to the user

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description); // if there are errors during user creation, add them to the model state to show validation errors in the view
                }
            }

            return View(model);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user);

            TempData["Success"] = "User deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}