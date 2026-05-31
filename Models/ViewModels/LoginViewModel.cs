using System.ComponentModel.DataAnnotations;

namespace PetcoSuppliesMVCAppLskinner.Models.ViewModels
{
    public class LoginViewModel
    {

        [Required]
        [EmailAddress] // email address of the user, marked as required and validated to ensure that it is provided and in a valid email format when submitting the login form
        public string Email { get; set; } // email address of the user, marked as required and validated to ensure that it is provided and in a valid email format when submitting the login form

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } // password of the user, marked as required and treated as a password field in the view to ensure that it is provided and handled securely when submitting the login form

        public bool RememberMe { get; set; } // remember me option, which is a boolean that indicates whether the user wants to be remembered on the device for future logins, and is not marked as required since it may be optional when submitting the login form
    }
}
