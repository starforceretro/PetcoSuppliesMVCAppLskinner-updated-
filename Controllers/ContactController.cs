using Microsoft.AspNetCore.Mvc;
using PetcoSuppliesMVCAppLskinner.Models.ViewModels;
using PetcoSuppliesMVCAppLskinner.Services;

namespace PetcoSuppliesMVCAppLskinner.Controllers
{
    public class ContactController : Controller
    {
        private readonly EmailService _emailService;

        public ContactController( // constructor with dependency injection
            EmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult AboutUs() // action method for the "About Us" page
        {
            return View();
        }
        public IActionResult Index() // display contact form
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index( // handle form submission
            ContactViewModel model)
        {
            if (ModelState.IsValid) // if the form data is valid, send the email
            {
                await _emailService.SendContactEmail( // send the contact email using the email service
                    model.Name,
                    model.Email,
                    model.Subject,
                    model.Message);

                TempData["Success"] = // set a success message to show after redirecting
                    "Your message has been sent.";

                return RedirectToAction(nameof(Index)); // redirect to the same page to show the success message and clear the form
            }

            return View(model); // if the form data is not valid, redisplay the form with validation errors
        }
    }
}