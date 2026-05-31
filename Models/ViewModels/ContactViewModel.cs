using System.ComponentModel.DataAnnotations;

namespace PetcoSuppliesMVCAppLskinner.Models.ViewModels
{
    public class ContactViewModel
    {
        [Required]
        public string Name { get; set; } // name of the person contacting, marked as required to ensure that it is provided when submitting the contact form

        [Required] // email address of the person contacting, marked as required to ensure that it is provided when submitting the contact form
        [EmailAddress] // email address of the person contacting, marked as required and validated to ensure that it is provided and in a valid email format when submitting the contact form
        public string Email { get; set; } // email address of the person contacting, marked as required and validated to ensure that it is provided and in a valid email format when submitting the contact form

        [Required]
        public string Subject { get; set; } // subject of the message, marked as required to ensure that it is provided when submitting the contact form

        [Required]
        public string Message { get; set; } // message content, marked as required to ensure that it is provided when submitting the contact form
    }
}