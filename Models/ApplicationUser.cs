namespace PetcoSuppliesMVCAppLskinner.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;
        public class ApplicationUser : IdentityUser
        {
            // basic info for the users

            [Required] // Data annotation to ensure that the FirstName property is required
            public string FirstName { get; set; }

            [Required] // Data annotation to ensure that the LastName property is required
            public string LastName { get; set; } // Data annotation to ensure that the LastName property is required

        [Required]
            public string Street { get; set; } // Data annotation to ensure that the Street property is required

        public string TelephoneNumber { get; set; } // Telephone number of the user, which is not marked as required since it may be optional when editing the profile

        [Required]
            public string Town { get; set; } // Data annotation to ensure that the Town property is required

        [Required]
            public string PostCode { get; set; } // Data annotation to ensure that the PostCode property is required

        public ICollection<Order> Orders { get; set; } // Navigation property to represent the one-to-many relationship between ApplicationUser and Order, allowing access to the user's orders through this property

    }
    }

