using System.ComponentModel.DataAnnotations;

namespace PetcoSuppliesMVCAppLskinner.Models
{
    public class Template
    {
        // this was originally going to bcrypt but i decided to use QuestPDF instead for invoice generation
        // if anyone ever wanted to do it with the original intended way, i will leave this here for reference, but it is not currently being used in the application

        [Key]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string TemplateName { get; set; }
        // HTML content with placeholders (e.g., {CustomerName}, {OrderDate}, {ProductList}, {TotalAmount})
        [Required]
        public string HtmlContent { get; set; }
        // The type of invoice template.
        public TemplateType TemplateType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
