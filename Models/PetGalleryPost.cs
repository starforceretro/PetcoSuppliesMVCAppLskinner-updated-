using PetcoSuppliesMVCAppLskinner.Models;

namespace PetcoSuppliesMVCAppLskinner.Models
{
    public class PetGalleryPost
    {

        public int Id { get; set; } // Primary key for the pet gallery post

        public string PetName { get; set; } // Name of the pet in the gallery post

        public string Description { get; set; } // Description of the pet or the story behind the photo

        public string ImageUrl { get; set; } // URL of the image of the pet in the gallery post

        public DateTime UploadDate { get; set; } // Date when the pet gallery post was uploaded

        public string UserId { get; set; } // Foreign key to the user who uploaded the pet gallery post

        public ApplicationUser User { get; set; } // Navigation property to the user who uploaded the pet gallery post
    }
}
