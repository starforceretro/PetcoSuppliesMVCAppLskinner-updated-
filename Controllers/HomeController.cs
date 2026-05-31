using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PetcoSuppliesMVCAppLskinner.Models;

namespace PetcoSuppliesMVCAppLskinner.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; // logger for logging information, warnings, and errors

        public HomeController(ILogger<HomeController> logger) // constructor with dependency injection for the logger
        {
            _logger = logger; // assign the injected logger to the private field
        }

        public IActionResult Index() // action method for the home page, which returns the default view
        {
            return View(); // return the view associated with this action (Views/Home/Index.cshtml)
        }

        public IActionResult Privacy() // action method for the privacy policy page, which returns the default view
        {
            return View(); // return the view associated with this action (Views/Home/Privacy.cshtml)
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // disable caching for the error page to ensure that it always shows the most up-to-date information
        public IActionResult Error() //     action method for the error page, which returns a view with an error model containing the request ID for debugging purposes
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // create a new instance of the ErrorViewModel and set the RequestId property to the current activity ID or the HTTP context trace identifier, then return the view with this model (Views/Home/Error.cshtml)
        }
    }
}
