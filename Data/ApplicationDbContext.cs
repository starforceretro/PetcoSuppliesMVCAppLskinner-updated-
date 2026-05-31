using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetcoSuppliesMVCAppLskinner.Models;

namespace PetcoSuppliesMVCAppLskinner.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } // DbSet for the Product entity, which represents the products available in the pet supplies store, allowing for CRUD operations on products in the database

        public DbSet<Category> Categories { get; set; } // DbSet for the Category entity, which represents the different categories of products in the pet supplies store, allowing for CRUD operations on categories in the database and enabling the organization of products into categories for easier browsing and filtering by customers

        public DbSet<Order> Orders { get; set; } // DbSet for the Order entity, which represents customer orders in the pet supplies store, allowing for CRUD operations on orders in the database and enabling the management of customer purchases, including order details, status, and history

        public DbSet<OrderItem> OrderItems { get; set; } // DbSet for the OrderItem entity, which represents individual items within a customer order in the pet supplies store, allowing for CRUD operations on order items in the database and enabling the association of products with specific orders, including quantity and price information for each item in an order

        public DbSet<Payment> Payments { get; set; } // DbSet for the Payment entity, which represents payment transactions for customer orders in the pet supplies store, allowing for CRUD operations on payments in the database and enabling the management of payment details, such as payment method, amount, and status for each order

        public DbSet<WishListItem> WishlistItems { get; set; } // DbSet for the WishListItem entity, which represents items that customers have added to their wishlist in the pet supplies store, allowing for CRUD operations on wishlist items in the database and enabling customers to save products they are interested in for future reference or purchase

        public DbSet<ReturnRequest> ReturnRequests { get; set; } // DbSet for the ReturnRequest entity, which represents customer requests to return products in the pet supplies store, allowing for CRUD operations on return requests in the database and enabling the management of return details, such as the product being returned, reason for return, status of the request, and associated order information

        public DbSet<PetGalleryPost> PetGalleryPosts { get; set; } // DbSet for the PetGalleryPost entity, which represents user-submitted posts in the pet gallery of the pet supplies store, allowing for CRUD operations on pet gallery posts in the database and enabling customers to share photos and stories of their pets, fostering community engagement and interaction on the website

        public DbSet<AdviceArticle> AdviceArticles { get; set; } // DbSet for the AdviceArticle entity, which represents informational articles in the advice section of the pet supplies store, allowing for CRUD operations on advice articles in the database and enabling the provision of helpful tips, guides, and information to customers about pet care, product usage, and other relevant topics to enhance their shopping experience and support responsible pet ownership

        public DbSet<Cart> Carts { get; set; } // DbSet for the Cart entity, which represents shopping carts for customers in the pet supplies store, allowing for CRUD operations on carts in the database and enabling customers to add products to their cart, view their cart contents, and proceed to checkout for their purchases

        public DbSet<CartItem> CartItems { get; set; } // DbSet for the CartItem entity, which represents individual items within a customer's shopping cart in the pet supplies store, allowing for CRUD operations on cart items in the database and enabling the association of products with specific carts, including quantity and price information for each item in a customer's cart

    }
}
