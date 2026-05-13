using PetcoSuppliesMVCAppLskinner.Models;

namespace PetcoSuppliesMVCAppLskinner.Models
{
    public class Order
    {
        
        public int Id { get; set; } // Primary key for the order

        public string UserId { get; set; } // Foreign key to the user who placed the order

        public ApplicationUser User { get; set; } // Navigation property to the user who placed the order

        public DateTime OrderDate { get; set; } // Date when the order was placed

        public decimal TotalAmount { get; set; } // Total amount for the order

        public string Status { get; set; } // Status of the order (e.g., Pending, Shipped, Delivered, Cancelled)

        public ICollection<OrderItem> OrderItems { get; set; } // Navigation property to the items in the order
    }
}
