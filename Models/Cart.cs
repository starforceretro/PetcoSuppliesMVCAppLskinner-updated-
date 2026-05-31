namespace PetcoSuppliesMVCAppLskinner.Models
{
    public class Cart
    {
            public int Id { get; set; } // unique identifier for the cart, which is an integer and serves as the primary key in the database

        public string UserId { get; set; } // foreign key to associate the cart with a specific user, which is a string that corresponds to the ID of the user in the ApplicationUser table, allowing for tracking which cart belongs to which user in the database

        public ApplicationUser User { get; set; } // navigation property to the ApplicationUser entity, which allows for easy access to the user information associated with the cart, enabling features such as displaying the user's name or email when viewing the cart contents

        public List<CartItem> CartItems { get; set; } // collection of CartItem entities that represent the individual items in the cart, allowing for easy management of the products added to the cart, including quantity and price information for each item, and enabling features such as calculating the total price of the cart or displaying the cart contents to the user

    }
}
