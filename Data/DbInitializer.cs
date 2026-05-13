using Microsoft.AspNetCore.Identity;
using PetcoSuppliesMVCAppLskinner.Models;
namespace PetcoSuppliesMVCAppLskinner.Data
{
    public static class DbInitializer
    {

        private static async Task CreateUserAsync(
       UserManager<ApplicationUser> userManager,
       string email,
       string password,
       string role,
       string firstName,
       string lastName,
        string street,
       string town,
       string postCode,
       string telephoneNumber)
        {
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName,
                    Street = street,
                    Town = town,
                    PostCode = postCode,
                    TelephoneNumber = telephoneNumber // <-- Add this line
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }

            }
        }

        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            var roleManager =
                service.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                service.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles =
            {
                "Admin",
                "Staff",
                "Customer"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }

            var adminEmail = "admin@petco.com";
            var adminPassword = "Admin123!";

            // Find existing user by email or username
            var adminUser = await userManager.FindByEmailAsync(adminEmail)
                            ?? await userManager.FindByNameAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    // Provide minimal required fields so creation won't fail if annotated as required
                    FirstName = "Admin",
                    LastName = "Account",
                    Street = "N/A",
                    Town = "N/A",
                    PostCode = "00000"
                };

                var result =
                    await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    adminUser = user;
                    await userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    // optional: log or inspect result.Errors
                    return;
                }
            }
            else
            {
                // Ensure existing user is in the Admin role
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }


            // ADMINS

            await CreateUserAsync(
                userManager,
                "admin@petco.com",
                "Admin123!",
                "Admin",
                "James",
                "Anderson",
                "12 King Street",
                "London",
                "SW1A1AA",
                "07111111111");

            await CreateUserAsync(
                userManager,
                "admin2@petco.com",
                "Admin123!",
                "Admin",
                "Sarah",
                "Wilson",
                "44 Rose Avenue",
                "Manchester",
                "M11AB",
                "07222222222");


            // STAFF

            await CreateUserAsync(
                userManager,
                "staff1@petco.com",
                "Staff123!",
                "Staff",
                "Emily",
                "Brown",
                "18 River Road",
                "Liverpool",
                "L12CD",
                "07333333333");

            await CreateUserAsync(
                userManager,
                "staff2@petco.com",
                "Staff123!",
                "Staff",
                "Daniel",
                "Taylor",
                "77 Green Lane",
                "Leeds",
                "LS11EF",
                "07444444444");

            await CreateUserAsync(
                userManager,
                "staff3@petco.com",
                "Staff123!",
                "Staff",
                "Olivia",
                "Martin",
                "5 Oak Close",
                "Birmingham",
                "B12GH",
                "07555555555");


            // CUSTOMERS

            await CreateUserAsync(
                userManager,
                "customer1@petco.com",
                "Customer123!",
                "Customer",
                "Alice",
                "Johnson",
                "91 High Street",
                "Glasgow",
                "G12JK",
                "07666666666");

            await CreateUserAsync(
                userManager,
                "customer2@petco.com",
                "Customer123!",
                "Customer",
                "Michael",
                "Smith",
                "23 Park Avenue",
                "Edinburgh",
                "EH34LM",
                "07777777777");

            await CreateUserAsync(
                userManager,
                "customer3@petco.com",
                "Customer123!",
                "Customer",
                "Chloe",
                "Davis",
                "8 Meadow Way",
                "Bristol",
                "BS56NO",
                "07888888888");

            await CreateUserAsync(
                userManager,
                "customer4@petco.com",
                "Customer123!",
                "Customer",
                "Ryan",
                "Walker",
                "14 Willow Drive",
                "Cardiff",
                "CF78PQ",
                "07999999999");

            await CreateUserAsync(
                userManager,
                "customer5@petco.com",
                "Customer123!",
                "Customer",
                "Sophia",
                "Evans",
                "66 Cedar Street",
                "Newcastle",
                "NE90RS",
                "07000000000");





        }
    }
}

