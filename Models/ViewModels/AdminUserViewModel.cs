using System.ComponentModel.DataAnnotations;

namespace PetcoSuppliesMVCAppLskinner.Models.ViewModels
{
    public class AdminUserViewModel
    {
        public string? Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string Town { get; set; }

        [Required]
        public string PostCode { get; set; }

        [Required]
        public string TelephoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        
        public string? Role { get; set; }

        
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string? ConfirmPassword { get; set; }
    }
}
