using PetcoSuppliesMVCAppLskinner.Models;

namespace PetcoSuppliesMVCAppLskinner.Models
{
    public class WishListItem
    {

        public int Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; } // Navigation property to the user who added the item to the wishlist

        public int ProductId { get; set; }

        public Product Product { get; set; } // Navigation property to the product that is in the wishlist
    }
}
