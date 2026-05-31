using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetcoSuppliesMVCAppLskinner.Models
{
    public class Product
    {
        public int Id { get; set; } // primary key for the Product entity

        [Required] // Data annotation to ensure that the Name property is required
        public string Name { get; set; } // name of the product, marked as required to ensure that it is provided when creating or editing a product

        public string Description { get; set; } // description of the product, which is not marked as required since it may be optional when creating or editing a product

        [Range(0.01, 99999)]  // Data annotation to ensure that the Price is a positive value
        public decimal Price { get; set; } // price of the product, marked with a range to ensure that it is a positive value when creating or editing a product

        public int Stock { get; set; } // stock quantity of the product, which is not marked as required since it may be optional when creating or editing a product (default value can be set to 0 if not provided)

        public string? ImageUrl { get; set; } // URL to the product image

        public int CategoryId { get; set; } // foreign key to the Category entity, which is not marked as required since it may be optional when creating or editing a product (default value can be set to 0 if not provided)

        public Category? Category { get; set; } // Navigation property to the related category

        [NotMapped]
        public IFormFile? ImageFile { get; set; } // property to handle the uploaded image file, marked with [NotMapped] to indicate that it should not be mapped to the database, and is nullable since it may not be provided when creating or editing a product
    }
}
