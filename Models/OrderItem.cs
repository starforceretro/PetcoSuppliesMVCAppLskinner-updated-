namespace PetcoSuppliesMVCAppLskinner.Models
{
    public class OrderItem
    {
        public int Id { get; set; } // Primary key for the order item

        public int OrderId { get; set; } // Foreign key to the order this item belongs to

        public Order Order { get; set; } // Navigation property to the order this item belongs to

        public int ProductId { get; set; } // Foreign key to the product being ordered

        public Product Product { get; set; } // Navigation property to the product being ordered

        public int Quantity { get; set; } // Quantity of the product being ordered

        public decimal Price { get; set; } // Price of the product at the time of the order (in case it changes later)

    }
}
