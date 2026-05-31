using Microsoft.AspNetCore.Identity;
using PetcoSuppliesMVCAppLskinner.Models;
namespace PetcoSuppliesMVCAppLskinner.Data
{
    public static class DbInitializer
    {

        private static async Task CreateUserAsync( // helper method to create a user with the specified details and role, checks if a user with the same email already exists to prevent duplicates, and if not, creates the user and assigns them to the specified role
       UserManager<ApplicationUser> userManager, // user manager for creating and managing users in the identity system, passed as a parameter to allow for dependency injection and reuse of the method for creating multiple users with different details and roles
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
            var existingUser = await userManager.FindByEmailAsync(email); // check if a user with the same email already exists in the database to prevent duplicate user accounts, which helps maintain data integrity and provides a better user experience by ensuring that each email address is associated with only one user account

            if (existingUser == null) // if no existing user is found with the same email, create a new user with the provided details and assign them to the specified role, which allows for the creation of new user accounts with different roles (e.g., Admin, Staff, Customer) and ensures that the new user is properly categorized in the system based on their role
            {
                var user = new ApplicationUser // create a new instance of the ApplicationUser class with the provided details, which will be used to create a new user account in the identity system
                {
                    UserName = email, // set the UserName property to the email address, which is a common practice in identity systems to use the email as the username for authentication purposes
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName,
                    Street = street,
                    Town = town,
                    PostCode = postCode,
                    TelephoneNumber = telephoneNumber // set the TelephoneNumber property to the provided telephone number, which allows for storing contact information for the user in the database and can be used for communication or verification purposes in the application
                };

                var result = await userManager.CreateAsync(user, password); // create the user in the database with the specified password, which will hash the password and store it securely in the database as part of the user account creation process

                if (result.Succeeded)// if the user creation is successful, assign the user to the specified role, which helps to categorize the user in the system and grant them the appropriate permissions based on their role (e.g., Admin, Staff, Customer)
                {
                    await userManager.AddToRoleAsync(user, role);
                }

            }
        }

        public static async Task SeedRolesAndAdminAsync(IServiceProvider service) // method to seed initial roles and an admin user into the database, checks for the existence of the specified roles and creates them if they do not exist, then checks for the existence of an admin user with a specific email and creates the user with the admin role if it does not exist, ensuring that there is at least one admin user in the system for managing the application
        {
            var roleManager = // get the RoleManager service from the service provider, which is used to manage roles in the identity system, allowing for the creation and management of roles such as Admin, Staff, and Customer
                service.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                service.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = // define an array of role names to be seeded into the database, which includes the roles "Admin", "Staff", and "Customer" that will be used to categorize users in the application and grant them appropriate permissions based on their role
            {
                "Admin",
                "Staff",
                "Customer"
            };

            foreach (var role in roles) // loop through each role in the array and check if it already exists in the database, if not, create the role using the RoleManager, which ensures that the necessary roles are available in the system for assigning to users and managing permissions based on their roles
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }

            var adminEmail = "admin@petco.com"; // define the email address for the admin user to be seeded into the database, which will be used as the username for authentication and to identify the admin user in the system
            var adminPassword = "Admin123!"; // define the password for the admin user to be seeded into the database, which will be used for authentication purposes and should meet the password requirements of the identity system (e.g., minimum length, complexity) to ensure that the admin account is secure

            // Find existing user by email or username
            var adminUser = await userManager.FindByEmailAsync(adminEmail)
                            ?? await userManager.FindByNameAsync(adminEmail);

            if (adminUser == null) // `if no existing user is found with the specified email or username, create a new admin user with the provided email and password, and assign them to the Admin role, which ensures that there is at least one admin user in the system for managing the application and performing administrative tasks
            {
                var user = new ApplicationUser // create a new instance of the ApplicationUser class for the admin user, setting the UserName and Email properties to the specified admin email, and providing minimal required fields to ensure that the user creation process does not fail if any of these fields are annotated as required in the ApplicationUser model
                {
                    UserName = adminEmail, // set the UserName property to the admin email, which is a common practice in identity systems to use the email as the username for authentication purposes
                    Email = adminEmail,
                    EmailConfirmed = true,
                    // Provide minimal required fields so creation won't fail if annotated as required
                    FirstName = "Admin",
                    LastName = "Account",
                    Street = "N/A",
                    Town = "N/A",
                    PostCode = "00000"
                };

                var result = // create the admin user in the database with the specified password, which will hash the password and store it securely in the database as part of the user account creation process
                    await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded) // if the user creation is successful, assign the user to the Admin role, which helps to categorize the user as an admin in the system and grant them the appropriate permissions for managing the application
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

            await CreateUserAsync( // seeding additional admin users with different details to ensure that there are multiple admin accounts in the system for redundancy and to allow for multiple administrators to manage the application, which can improve security and provide better coverage for administrative tasks
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

            await CreateUserAsync( // seeding staff users with different details to ensure that there are staff accounts in the system for managing day-to-day operations and providing support to customers, which can improve the overall functionality and user experience of the application by allowing staff members to perform their roles effectively
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

            await CreateUserAsync( // seeding customer users with different details to ensure that there are customer accounts in the system for testing and demonstrating the functionality of the application from a customer's perspective, which can help to improve the overall user experience and allow for better testing of customer-related features such as browsing products, placing orders, and managing their accounts
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

