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
    string lastName)
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
                    Street = "123 Test Street",
                    Town = "London",
                    PostCode = "AB12CD"
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }

                await CreateUserAsync(
    userManager,
    "staff1@petco.com",
    "Staff123!",
    "Staff",
    "Jane",
    "Smith");

                await CreateUserAsync(
                    userManager,
                    "customer1@petco.com",
                    "Customer123!",
                    "Customer",
                    "Bob",
                    "Jones");

                await CreateUserAsync(
                    userManager,
                    "customer2@petco.com",
                    "Customer123!",
                    "Customer",
                    "Alice",
                    "Brown");
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
        }




    }
}

