using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetcoSuppliesMVCAppLskinner.Data;
using PetcoSuppliesMVCAppLskinner.Models;

namespace PetcoSuppliesMVCAppLskinner.Controllers
{
    [Authorize]
    public class ReturnsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReturnsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // customer view 

        public async Task<IActionResult> MyReturns()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value; // get the current user's ID from the claims, which is used to filter the return requests to only show those that belong to the logged-in user

            var returns = await _context.ReturnRequests // query the database for return requests that belong to the current user, including related product and order information, and convert the results to a list asynchronously
                .Include(r => r.Product)
                .Include(r => r.Order)
                .Where(r => r.Order.UserId == userId)
                .ToListAsync();

            return View(returns); // pass the list of return requests to the view (Views/Returns/MyReturns.cshtml) to display the user's return requests
        }

        // create return request

        public IActionResult Create(int orderId, int productId) // action method to display the form for creating a new return request, accepts the order ID and product ID as parameters, creates a new ReturnRequest model with the provided order and product IDs, and passes it to the view to pre-fill the form fields for the return request
        {
            var model = new ReturnRequest // create a new instance of the ReturnRequest model to pass to the view, initializing the OrderId and ProductId properties with the values passed as parameters to pre-fill the form fields in the view
            {
                OrderId = orderId,
                ProductId = productId
            };

            return View(model); // return the view for creating a new return request (Views/Returns/Create.cshtml) with the initialized model to pre-fill the form fields for the order and product IDs
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // action method to handle the form submission for creating a new return request, accepts a ReturnRequest model as a parameter, performs validation and security checks, and if everything is valid, saves the new return request to the database and redirects the user back to the MyReturns view with a success message
        public async Task<IActionResult> Create(ReturnRequest request) // action method to handle the form submission for creating a new return request, accepts a ReturnRequest model as a parameter, performs validation and security checks, and if everything is valid, saves the new return request to the database and redirects the user back to the MyReturns view with a success message
        {
            // DEBUGGING

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine(error.Key);

                    foreach (var subError in error.Value.Errors)
                    {
                        Console.WriteLine(subError.ErrorMessage);
                    }
                }

                return View(request);
            }

            // get current user ID

            var userId =
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // check if order exists

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == request.OrderId);

            if (order == null)
            {
                return NotFound();
            }

            // security check: ensure the order belongs to the current user

            if (order.UserId != userId)
            {
                return Unauthorized();
            }

            // checkc if a return request for this order and product already exists to prevent duplicate requests for the same item

            var existingReturn = await _context.ReturnRequests // query the database for an existing return request that matches the order ID and product ID of the new request being created, which helps to ensure that customers cannot submit multiple return requests for the same item in the same order
                .FirstOrDefaultAsync(r =>
                    r.OrderId == request.OrderId &&
                    r.ProductId == request.ProductId);

            if (existingReturn != null) // if an existing return request is found for the same order and product, set an error message in TempData and redirect the user back to the MyReturns view to inform them that a return request for this item has already been submitted, preventing duplicate requests and ensuring a better user experience
            {
                TempData["Error"] =
                    "A return request for this item has already been submitted.";

                return RedirectToAction(nameof(MyReturns));
            }

            // set additional properties for the return request

            request.Status = "Pending";

            request.RequestDate = DateTime.Now;

            // save 

            _context.ReturnRequests.Add(request);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Return request submitted successfully.";

            return RedirectToAction(nameof(MyReturns));
        }

        // staff view of all return requests

        [Authorize(Roles = "Admin,Staff")] // only allow staff and admin to access this view
        public async Task<IActionResult> Index() // action method to display all return requests, retrieves all return requests from the database, including related product and order information, ordered by request date, and passes them to the view
        {
            var requests = await _context.ReturnRequests // get all return requests from the database
    .Include(r => r.Product)
    .Include(r => r.Order)
        .ThenInclude(o => o.User)
    .OrderByDescending(r => r.RequestDate)
    .ToListAsync();

            return View(requests); // pass the list of return requests to the view (Views/Returns/Index.cshtml)
        }

        // approve return request

        [Authorize(Roles = "Admin,Staff")] // only allow staff and admin to approve return requests
        public async Task<IActionResult> Approve(int id) // action method to approve a return request, accepts the ID of the return request to be approved as a parameter, retrieves the return request from the database, and if it exists, updates its status to "Approved" and saves the changes before redirecting back to the Index action to display the updated list of return requests
        {
            var request = await _context.ReturnRequests.FindAsync(id); // find the return request by id

            if (request != null) // if the return request exists, update its status to "Approved" and save the changes to the database
            {
                request.Status = "Approved"; // update the status of the return request to "Approved"

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)); // after approving the return request, redirect back to the Index action to display the updated list of return requests with the approved request now showing as "Approved"
        }

        // REJECT

        [Authorize(Roles = "Admin,Staff")] // only allow staff and admin to reject return requests
        public async Task<IActionResult> Reject(int id) // action method to reject a return request, accepts the ID of the return request to be rejected as a parameter, retrieves the return request from the database, and if it exists, updates its status to "Rejected" and saves the changes before redirecting back to the Index action to display the updated list of return requests
        {
            var request = await _context.ReturnRequests.FindAsync(id); // find the return request by id

            if (request != null)
            {
                request.Status = "Rejected";

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)); // after rejecting the return request, redirect back to the Index action to display the updated list of return requests with the rejected request now showing as "Rejected"
        }
    }
}