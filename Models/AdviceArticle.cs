
using System.ComponentModel.DataAnnotations;

namespace PetcoSuppliesMVCAppLskinner.Models
{
    public class AdviceArticle
    {
        public int Id { get; set; } // unique id for each post

        [Required]
        public string Title { get; set; } // title of the advice article

        [Required]
        public string Content { get; set; } // main content of the advice article

        [Required]
        public string AnimalCategory { get; set; } // category of animal the advice is about (e.g., dogs, cats, birds)

        public DateTime CreatedDate { get; set; } // date the advice article was created
    }
}
