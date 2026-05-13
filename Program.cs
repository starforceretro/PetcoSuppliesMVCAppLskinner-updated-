using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetcoSuppliesMVCAppLskinner.Data;
using PetcoSuppliesMVCAppLskinner.Models;

namespace PetcoSuppliesMVCAppLskinner
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            //builder.Services.AddControllersWithViews();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

            builder.Services.AddAuthorization();
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // ADD THIS
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                await DbInitializer.SeedRolesAndAdminAsync(services);
            }

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                context.Database.Migrate();

                // SEED CATEGORIES
                if (!context.Categories.Any())
                {
                    var dogCategory = new Category
                    {
                        Name = "Dogs"
                    };

                    var catCategory = new Category
                    {
                        Name = "Cats"
                    };

                    var fishCategory = new Category
                    {
                        Name = "Fish"
                    };

                    var reptileCategory = new Category
                    {
                        Name = "Reptiles"
                    };

                    var smallAnimalCategory = new Category
                    {
                        Name = "Small Animals"
                    };

                    context.Categories.AddRange(
                        dogCategory,
                        catCategory,
                        fishCategory,
                        reptileCategory,
                        smallAnimalCategory
                    );

                    context.SaveChanges();

                    // SEED PRODUCTS
                    context.Products.AddRange(

                        new Product
                        {
                            Name = "Premium Dog Food",
                            Description = "Nutritious dry food for dogs.",
                            Price = 24.99m,
                            Stock = 15,
                            ImageUrl = "/images/dogfood.png",
                            CategoryId = dogCategory.Id
                        },

                        new Product
                        {
                            Name = "Cat Scratching Post",
                            Description = "Tall scratching post for cats.",
                            Price = 39.99m,
                            Stock = 8,
                            ImageUrl = "/images/catscratch.png",
                            CategoryId = catCategory.Id
                        },

                        new Product
                        {
                            Name = "Fish Tank Starter Kit",
                            Description = "Complete aquarium starter kit.",
                            Price = 89.99m,
                            Stock = 5,
                            ImageUrl = "/images/fishtank.png",
                            CategoryId = fishCategory.Id
                        },

                        new Product
                        {
                            Name = "Snake Heat Lamp",
                            Description = "Heat lamp for reptile enclosures.",
                            Price = 29.99m,
                            Stock = 12,
                            ImageUrl = "/images/heatlamp.png",
                            CategoryId = reptileCategory.Id
                        },

                        new Product
                        {
                            Name = "Hamster Wheel",
                            Description = "Silent exercise wheel for hamsters.",
                            Price = 12.99m,
                            Stock = 20,
                            ImageUrl = "/images/hamsterwheel.jpg",
                            CategoryId = smallAnimalCategory.Id
                        }
                    );

                    context.SaveChanges();
                }
            }

            app.Run();
        }
    }
}
