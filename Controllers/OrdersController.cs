using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetcoSuppliesMVCAppLskinner.Data;
using PetcoSuppliesMVCAppLskinner.Models;
using PetcoSuppliesMVCAppLskinner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PetcoSuppliesMVCAppLskinner.Services;

namespace PetcoSuppliesMVCAppLskinner.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context; // database context

        private readonly UserManager<ApplicationUser> _userManager; // user manager for handling user-related operations

        private readonly InvoiceService _invoiceService; // service for generating PDF invoices
        private readonly EmailService _emailService; // service for sending emails

        public OrdersController( // constructor with dependency injection
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            InvoiceService invoiceService,
            EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _invoiceService = invoiceService;
            _emailService = emailService;
        }

        public async Task<IActionResult> DownloadInvoice(int id) // action to download PDF invoice for a specific order
        {
            var order = await _context.Orders // get the order from the database, including related user and order items
                .Include(o => o.User) // include user details for the invoice
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) // if the order does not exist, return a 404 Not Found response
            {
                return NotFound();
            }

            var pdfBytes =
                _invoiceService.GenerateInvoice(order); // generate the PDF invoice as a byte array

            return File( // return the PDF file as a downloadable response
                pdfBytes,
                "application/pdf",
                $"Invoice_Order_{order.Id}.pdf");
        }

        [Authorize] // action to handle the checkout process for the user's cart
        public async Task<IActionResult> Checkout() // this action will create an order from the user's cart, reduce stock, clear the cart, and send a confirmation email
        {
            var user =
                await _userManager.GetUserAsync(User); // get the currently logged-in user

            // get the user's cart with items and product details

            var cart = await _context.Carts // find the cart for the current user
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == user.Id); // include cart items and product details for stock checking and order creation

            if (cart == null || !cart.CartItems.Any()) // if the cart is null or empty, redirect back to the cart page with an error message
            {
                TempData["Error"] = "Your cart is empty."; // set an error message to show on the cart page

                return RedirectToAction( // redirect to the cart page
                    "Index",
                    "Cart");
            }

            // check stock for each item in the cart before creating the order

            foreach (var item in cart.CartItems) // loop through each cart item to check if there is enough stock for the product
            {
                if (item.Quantity > item.Product.Stock) // if the quantity in the cart exceeds the available stock, redirect back to the cart page with an error message
                {
                    TempData["Error"] =
                        $"{item.Product.Name} does not have enough stock."; // set an error message indicating which product is out of stock

                    return RedirectToAction( // redirect to the cart page
                        "Index",
                        "Cart");
                }
            }

            // create a new order with the cart items and calculate the total amount

            var order = new Order // create a new order object with the user ID, current date, pending status, and an empty list of order items
            {
                UserId = user.Id, // set the user ID for the order
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = 0,
                OrderItems = new List<OrderItem>()
            };

            decimal total = 0;

            // convert each cart item to an order item, add it to the order, reduce stock, and calculate the total amount for the order

            foreach (var item in cart.CartItems)
            {
                var orderItem = new OrderItem // create a new order item object for each cart item, setting the product ID, quantity, and price
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                };

                order.OrderItems.Add(orderItem);

                // reduce stock for the product based on the quantity ordered

                item.Product.Stock -= item.Quantity; // reduce the stock of the product by the quantity ordered

                total += item.Product.Price * item.Quantity; // calculate the total amount for the order by summing the price of each order item (price * quantity)
            }

            order.TotalAmount = total; // set the total amount for the order after calculating it from the order items

            _context.Orders.Add(order); // add the new order to the database context

            // clear the user's cart after creating the order

            _context.CartItems.RemoveRange(cart.CartItems); // remove all cart items from the database context to clear the cart

            await _context.SaveChangesAsync(); // save all changes to the database, including the new order, stock reduction, and cart clearance

            await _emailService.SendOrderConfirmationEmail( // send an order confirmation email to the user with the order details
    user.Email,
    order);

            return RedirectToAction( // redirect to the order confirmation page to show the details of the newly created order
                nameof(OrderConfirmation),
                new { id = order.Id });

            return RedirectToAction(
                nameof(OrderConfirmation),
                new { id = order.Id });
        }

        // shows user orders 
        [Authorize]
        public async Task<IActionResult> UserOrders() // action to display the current user's past orders
        {
            var user =
                await _userManager.GetUserAsync(User); // get the currently logged-in user

            var orders = await _context.Orders // query the database for orders that belong to the current user, including order items and product details, and order them by date
                .Where(o => o.UserId == user.Id)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders); // pass the list of orders to the view to display them to the user
        }

        // confirmation page 

        public async Task<IActionResult> OrderConfirmation(int? id) // action to display the order confirmation page with the details of the newly created order
        {
            if (id == null) // if the order ID is not provided, return a 404 Not Found response
            {
                return NotFound();
            }
            var order = await _context.Orders // query the database for the order with the specified ID, including user details and order items with product details, to show on the confirmation page
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // GET: Orders
        public async Task<IActionResult> Index() // action to display a list of all orders (for admin view)
        {
            var applicationDbContext = _context.Orders.Include(o => o.User); // query the database for all orders, including user details, to show in the admin view
            return View(await applicationDbContext.ToListAsync());
        }



        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id) // action to display the details of a specific order (for admin view)
        {
            if (id == null) // if the order ID is not provided, return a 404 Not Found response
            {
                return NotFound();
            }

            var order = await _context.Orders // query the database for the order with the specified ID, including user details, to show in the admin view
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create() // action to display the order creation form (for admin view)
        {
            ViewData["UserId"] = new SelectList(
     _context.Users.Select(u => new
     {
         Id = u.Id,
         Display = u.Email
     }),
     "Id",
     "Display"
 );

            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,OrderDate,TotalAmount,Status")] Order order) // action to handle the submission of the order creation form (for admin view)
        {
            Console.WriteLine("CREATE POST HIT"); // debugging statement to confirm the action is being hit

            Console.WriteLine($"UserId = {order.UserId}");
            Console.WriteLine($"OrderDate = {order.OrderDate}");
            Console.WriteLine($"TotalAmount = {order.TotalAmount}");
            Console.WriteLine($"Status = {order.Status}");

            if (ModelState.IsValid) // if the model state is valid, save the new order to the database
            {
                Console.WriteLine("MODEL VALID");

                _context.Add(order);
                await _context.SaveChangesAsync();

                Console.WriteLine("ORDER SAVED");

                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine("MODEL INVALID"); // debugging statement to indicate the model state is invalid

            foreach (var item in ModelState)
            {
                foreach (var error in item.Value.Errors)
                {
                    Console.WriteLine($"{item.Key}: {error.ErrorMessage}");
                }
            }

            ViewData["UserId"] =
                new SelectList(_context.Users, "Id", "Id", order.UserId);

            return View(order);
        }
        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id) // action to display the order editing form for a specific order (for admin view)
        {
            if (id == null) // if the order ID is not provided, return a 404 Not Found response
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id); // query the database for the order with the specified ID to populate the editing form

            if (order == null) // if the order is not found, return a 404 Not Found response
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList( // populate the dropdown list for selecting a user in the editing form with user IDs and emails for display
                _context.Users.Select(u => new
                {
                    Id = u.Id,
                    Display = u.Email
                }),
                "Id",
                "Display"
            );

            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,OrderDate,TotalAmount,Status")] Order order) // action to handle the submission of the order editing form for a specific order (for admin view)
        {
            if (id != order.Id) // if the order ID in the URL does not match the order ID in the form data, return a 404 Not Found response
            {
                return NotFound();
            }

            if (ModelState.IsValid) // if the model state is valid, update the order in the database
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["UserId"] = new SelectList(
                _context.Set<ApplicationUser>(),
                "Id",
                "Id",
                order.UserId);

            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id) // action to display the order deletion confirmation page for a specific order (for admin view)
        {
            if (id == null)
            {
                return NotFound(); // if the order ID is not provided, return a 404 Not Found response
            }

            var order = await _context.Orders // query the database for the order with the specified ID, including user details, to show on the deletion confirmation page
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order); // pass the order to the view to display the details of the order being deleted and confirm the deletion action
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) // action to handle the submission of the order deletion confirmation form for a specific order (for admin view)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }


}
