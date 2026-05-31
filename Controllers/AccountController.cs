using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PetcoSuppliesMVCAppLskinner.Models;
using PetcoSuppliesMVCAppLskinner.Models.ViewModels;

namespace PetcoSuppliesMVCAppLskinner.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager; // inject user manager
        private readonly SignInManager<ApplicationUser> _signInManager; // inject sign in manager
        public AccountController(UserManager<ApplicationUser> userManager, // constructor
                                      SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index() // account overview
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> EditProfile() // load edit profile form with current user details
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var model = new EditProfileViewModel // populate view model with current user details
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
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile( // handle profile update
    EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            // Update user properties with form values

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;

            user.Street = model.Street;
            user.Town = model.Town;
            user.PostCode = model.PostCode;
            user.TelephoneNumber = model.TelephoneNumber;

            // Optional password change

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var token =
                    await _userManager.GeneratePasswordResetTokenAsync(user);

                var passwordResult = // reset password using token and new password
                    await _userManager.ResetPasswordAsync(
                        user,
                        token,
                        model.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError(
                            "",
                            error.Description); // add password reset errors to model state
                    }

                    return View(model);
                }
            }

            var result =
                await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] =
                    "Profile updated successfully."; // success message

                return RedirectToAction("Index", "Home"); // redirect to home page after successful update
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    "",
                    error.Description);
            }

            return View(model);
        }

        public IActionResult Register() // load registration form
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model) // handle registration form submission
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser // create new user with form values
                {
                    UserName = model.Email, // use email as username
                    Email = model.Email, // set email
                    FirstName = model.FirstName, // set first name
                    LastName = model.LastName, // set last name
                    Street = model.Street, // set street
                    TelephoneNumber = model.TelephoneNumber, // set telephone number
                    Town = model.Town, // set town
                    PostCode = model.PostCode // set post code
                };
                var result = await _userManager.CreateAsync(user, model.Password); // create user with password
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(model);
        }

        public IActionResult Login() // load login form
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model) // handle login form submission
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, // use email as username for login
                    model.Password, // password
                    model.RememberMe, // remember me option
                    lockoutOnFailure: false); // do not lock out on failure

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home"); // redirect to home page on successful login
                }

                ModelState.AddModelError("", "Invalid login attempt"); // add error message for failed login
            }

            return View(model); // return to login form if model state is invalid or login fails
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() // handle logout action
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home"); // redirect to home page after logout
        }
    }
}
