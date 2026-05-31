using System.ComponentModel.DataAnnotations;

namespace PetcoSuppliesMVCAppLskinner.Models.ViewModels
{
    public class EditProfileViewModel
    {
        public string Id { get; set; } // user ID, which is a string in ASP.NET Identity, used to identify the user being edited

        [Required]
        public string FirstName { get; set; } // first name of the user, marked as required to ensure that it is provided when editing the profile

        [Required]
        public string LastName { get; set; } // last name of the user, marked as required to ensure that it is provided when editing the profile

        [Required]
        [EmailAddress]
        public string Email { get; set; } // email address of the user, marked as required and validated to ensure that it is provided and in a valid email format when editing the profile

        [Required]
        public string Street { get; set; } //   street address of the user, marked as required to ensure that it is provided when editing the profile

        [Required]
        public string Town { get; set; } // town or city of the user, marked as required to ensure that it is provided when editing the profile

        [Required]
        public string PostCode { get; set; } // postal code of the user's address, marked as required to ensure that it is provided when editing the profile

        public string TelephoneNumber { get; set; } //  telephone number of the user, which is not marked as required since it may be optional when editing the profile

        // OPTIONAL PASSWORD CHANGE

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; } // new password for the user, marked as a password data type to ensure that it is treated as a password field in the view, but is not marked as required since it may be optional when editing the profile (only required when changing the password)

        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string? ConfirmNewPassword { get; set; } // confirmation of the new password, marked as a password data type to ensure that it is treated as a password field in the view, and uses the Compare attribute to ensure that it matches the NewPassword field when editing the profile (only required when changing the password)
    }
}

