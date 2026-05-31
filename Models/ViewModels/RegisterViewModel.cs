using System.ComponentModel.DataAnnotations;

namespace PetcoSuppliesMVCAppLskinner.Models.ViewModels
{
    public class RegisterViewModel
    {

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } // first name of the user, marked as required to ensure that it is provided when submitting the registration form, and has a maximum length of 50 characters to prevent excessively long input

        [Required] // last name of the user, marked as required to ensure that it is provided when submitting the registration form, and has a maximum length of 50 characters to prevent excessively long input
        [StringLength(50)] //   last name of the user, marked as required to ensure that it is provided when submitting the registration form, and has a maximum length of 50 characters to prevent excessively long input
        public string LastName { get; set; } // last name of the user, marked as required to ensure that it is provided when submitting the registration form, and has a maximum length of 50 characters to prevent excessively long input

        [Required]
        [StringLength(100)] // street address of the user, marked as required to ensure that it is provided when submitting the registration form, and has a maximum length of 100 characters to prevent excessively long input
        public string Street { get; set; } // street address of the user, marked as required to ensure that it is provided when submitting the registration form, and has a maximum length of 100 characters to prevent excessively long input

        [Required]
        [StringLength(50)] // town or city of the user, marked as required to ensure that it is provided when submitting the registration form, and has a maximum length of 50 characters to prevent excessively long input
        public string Town { get; set; } // town or city of the user, marked as required to ensure that it is provided when submitting the registration form, and has a maximum length of 50 characters to prevent excessively long input

        [Required]
        [StringLength(20)] // postal code of the user's address, marked as required to ensure that it is provided when submitting the registration form, and has a maximum length of 20 characters to accommodate various postal code formats while preventing excessively long input
        public string PostCode { get; set; } // postal code of the user's address, marked as required to ensure that it is provided when submitting the registration form, and has a maximum length of 20 characters to accommodate various postal code formats while preventing excessively long input

        [Phone] // telephone number of the user, marked with the [Phone] attribute to validate that the input is in a valid phone number format, but not marked as required to allow users to register without providing a phone number if they choose
        public string TelephoneNumber { get; set; } // telephone number of the user, marked with the [Phone] attribute to validate that the input is in a valid phone number format, but not marked as required to allow users to register without providing a phone number if they choose

        [Required]
        [EmailAddress] // email address of the user, marked as required and validated to ensure that it is provided and in a valid email format when submitting the registration form
        public string Email { get; set; } // email address of the user, marked as required and validated to ensure that it is provided and in a valid email format when submitting the registration form

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } // password of the user, marked as required and treated as a password field in the view to ensure that it is provided and handled securely when submitting the registration form, and has a minimum length of 6 characters and a maximum length of 100 characters to enforce password strength requirements while preventing excessively long input

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")] // confirmation of the password, marked as required and treated as a password field in the view to ensure that it is provided and handled securely when submitting the registration form, and uses the Compare attribute to ensure that it matches the Password field, with a custom error message if they do not match
        public string ConfirmPassword { get; set; } // confirmation of the password, marked as required and treated as a password field in the view to ensure that it is provided and handled securely when submitting the registration form, and uses the Compare attribute to ensure that it matches the Password field, with a custom error message if they do not match
    }
}
