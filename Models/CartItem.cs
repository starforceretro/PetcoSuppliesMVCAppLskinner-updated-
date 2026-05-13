namespace PetcoSuppliesMVCAppLskinner.Models
{
    public class CartItem
    {
        // This class represents an item in the shopping cart. It contains properties for the product ID, name, price, quantity, and image URL.
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string ImageUrl { get; set; }
    }
}
