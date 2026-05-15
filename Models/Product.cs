using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetcoSuppliesMVCAppLskinner.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required] // Data annotation to ensure that the Name property is required
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(0.01, 99999)]  // Data annotation to ensure that the Price is a positive value
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string? ImageUrl { get; set; } // URL to the product image

        public int CategoryId { get; set; }

        public Category? Category { get; set; } // Navigation property to the related category

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
