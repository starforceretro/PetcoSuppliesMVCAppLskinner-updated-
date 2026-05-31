namespace PetcoSuppliesMVCAppLskinner.Models
{
    public class CartItem
    {
        public int Id { get; set; } // unique identifier for the cart item, used as the primary key in the database

        public int CartId { get; set; } // foreign key to the Cart entity, which represents the shopping cart that this item belongs to, allowing for the association of multiple cart items with a single cart in the database and enabling the retrieval of all items in a customer's cart when needed

        public Cart Cart { get; set; } // navigation property to the Cart entity, which allows for easy access to the cart that this item belongs to when working with the data in the application, enabling operations such as retrieving the cart details or calculating the total price of the items in the cart

        public int ProductId { get; set; } // foreign key to the Product entity, which represents the product that this cart item refers to, allowing for the association of a specific product with this cart item in the database and enabling the retrieval of product details such as name, price, and description when needed

        public Product Product { get; set; } // navigation property to the Product entity, which allows for easy access to the product details that this cart item refers to when working with the data in the application, enabling operations such as displaying the product information in the cart or calculating the total price based on the product's price and quantity

        public int Quantity { get; set; } // the quantity of the product that the customer has added to their cart, which is used to calculate the total price for this cart item and to manage inventory levels when processing orders, allowing customers to specify how many units of a product they want to purchase and enabling the application to handle multiple quantities of the same product in the cart
    }
}
