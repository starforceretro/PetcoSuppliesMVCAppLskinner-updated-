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

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<WishListItem> WishlistItems { get; set; }

        public DbSet<ReturnRequest> ReturnRequests { get; set; }

        public DbSet<PetGalleryPost> PetGalleryPosts { get; set; }

        public DbSet<AdviceArticle> AdviceArticles { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }
    }
}
