using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetcoSuppliesMVCAppLskinner.Data;

namespace PetcoSupplies.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.ProductCount = _context.Products.Count();

            ViewBag.OrderCount = _context.Orders.Count();

            ViewBag.CategoryCount = _context.Categories.Count();

            return View();
        }
    }
}