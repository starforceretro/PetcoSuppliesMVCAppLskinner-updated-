namespace PetcoSuppliesMVCAppLskinner.Models
{
    public class Category
    {
        public int Id { get; set; } // Primary key for the Category entity, which is an integer that uniquely identifies each category in the database and is used for relationships with other entities (e.g., Product)

        public string Name { get; set; } // Name of the category, which is a string that represents the name of the category (e.g., "Dog Supplies", "Cat Supplies") and is used for display purposes in the application

        public ICollection<Product> Products { get; set; }// Navigation property to related products
    }
}
