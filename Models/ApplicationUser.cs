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
            public string LastName { get; set; }

            [Required]
            public string Street { get; set; }
            
            public string TelephoneNumber { get; set; }

            [Required]
            public string Town { get; set; }

            [Required]
            public string PostCode { get; set; }

          

        }
    }

