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

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;

            _userManager = userManager;

            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();

            return View(users);
        }


        public IActionResult CreateUser()
        {
            return View();
        }

        public async Task<IActionResult> Details(string id) // a page to show the details of a user, including their orders
        {
            var user = await _context.Users
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
                Id = user.Id,
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
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
                return NotFound();

            // Update user details

            user.FirstName = model.FirstName;
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
                    await _userManager.GeneratePasswordResetTokenAsync(user);

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

            // SAVE USER CHANGES

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(AdminUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Street = model.Street,
                    Town = model.Town,
                    PostCode = model.PostCode,
                    TelephoneNumber = model.TelephoneNumber,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(
                    user,
                    model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);

        }
        

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}