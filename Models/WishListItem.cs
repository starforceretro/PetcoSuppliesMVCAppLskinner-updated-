using PetcoSuppliesMVCAppLskinner.Models;

namespace PetcoSuppliesMVCAppLskinner.Models
{
    public class WishListItem
    {

        public int Id { get; set; } // unique identifier for each wishlist item, which is used as the primary key in the database to distinguish between different items in the wishlist and to perform operations such as adding, removing, or retrieving specific wishlist items based on their ID

        public string UserId { get; set; } // foreign key to the user who added the item to the wishlist, which is used to associate each wishlist item with a specific user in the database, allowing for retrieval of wishlist items based on the user and ensuring that each user's wishlist is personalized and secure

        public ApplicationUser User { get; set; } // Navigation property to the user who added the item to the wishlist

        public int ProductId { get; set; } // foreign key to the product that is in the wishlist, which is used to associate each wishlist item with a specific product in the database, allowing for retrieval of product details based on the wishlist item and enabling users to view information about the products they have added to their wishlist

        public Product Product { get; set; } // Navigation property to the product that is in the wishlist
    }
}
