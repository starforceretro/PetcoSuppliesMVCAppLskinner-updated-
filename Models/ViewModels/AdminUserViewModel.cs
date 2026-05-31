using System.ComponentModel.DataAnnotations;

namespace PetcoSuppliesMVCAppLskinner.Models.ViewModels
{
    public class AdminUserViewModel
    {
        public string? Id { get; set; } // user ID, which is a string in ASP.NET Identity, used to identify the user being edited or created

        [Required] // first name of the user, marked as required to ensure that it is provided when creating or editing a user
        public string FirstName { get; set; } // first name of the user, marked as required to ensure that it is provided when creating or editing a user

        [Required] // last name of the user, marked as required to ensure that it is provided when creating or editing a user
        public string LastName { get; set; } // last name of the user, marked as required to ensure that it is provided when creating or editing a user

        [Required]
        public string Street { get; set; } // street address of the user, marked as required to ensure that it is provided when creating or editing a user 

        [Required]
        public string Town { get; set; } // town or city of the user, marked as required to ensure that it is provided when creating or editing a user

        [Required]
        public string PostCode { get; set; } // postal code of the user's address, marked as required to ensure that it is provided when creating or editing a user

        [Required]
        public string TelephoneNumber { get; set; } // telephone number of the user, marked as required to ensure that it is provided when creating or editing a user

        [Required]
        [EmailAddress] // email address of the user, marked as required and validated to ensure that it is provided and in a valid email format when creating or editing a user
        public string Email { get; set; } // email address of the user, marked as required and validated to ensure that it is provided and in a valid email format when creating or editing a user


        public string? Role { get; set; } // role of the user (e.g., Admin, Customer), which can be used to assign or display the user's role in the application, but is not marked as required since it may be optional when editing a user


        [DataType(DataType.Password)] // current password of the user, marked as a password data type to ensure that it is treated as a password field in the view, but is not marked as required since it may be optional when editing a user (only required when changing the password)
        public string? Password { get; set; } // current password of the user, marked as a password data type to ensure that it is treated as a password field in the view, but is not marked as required since it may be optional when editing a user (only required when changing the password)

        [DataType(DataType.Password)] // new password for the user, marked as a password data type to ensure that it is treated as a password field in the view, but is not marked as required since it may be optional when editing a user (only required when changing the password)
        public string? NewPassword { get; set; } // new password for the user, marked as a password data type to ensure that it is treated as a password field in the view, but is not marked as required since it may be optional when editing a user (only required when changing the password)

        [DataType(DataType.Password)] // confirmation of the new password, marked as a password data type to ensure that it is treated as a password field in the view, and uses the Compare attribute to ensure that it matches the NewPassword field when creating or editing a user (only required when changing the password)
        [Compare("NewPassword")]
        public string? ConfirmPassword { get; set; } // confirmation of the new password, marked as a password data type to ensure that it is treated as a password field in the view, and uses the Compare attribute to ensure that it matches the NewPassword field when creating or editing a user (only required when changing the password)
    }
}
