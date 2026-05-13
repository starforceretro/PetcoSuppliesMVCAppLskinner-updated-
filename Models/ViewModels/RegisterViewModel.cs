using System.ComponentModel.DataAnnotations;

namespace PetcoSuppliesMVCAppLskinner.Models.ViewModels
{
    public class RegisterViewModel
    {

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100)]
        public string Street { get; set; }

        [Required]
        [StringLength(50)]
        public string Town { get; set; }

        [Required]
        [StringLength(20)]
        public string PostCode { get; set; }

        [Phone]
        public string TelephoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
