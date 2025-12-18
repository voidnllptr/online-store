using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using online_store.Models;
using System.Text;

namespace online_store.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        using var context = scopedProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scopedProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scopedProvider.GetRequiredService<UserManager<IdentityUser>>();

        await context.Database.EnsureCreatedAsync();

        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Electronic" },
                new Category { Name = "Food" },
                new Category { Name = "Home" },
                new Category { Name = "Toy" }
            };
            
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        if (!context.Products.Any())
        {
            var categories = await context.Categories.ToListAsync();
            var electronic = categories.FirstOrDefault(c => c.Name == "Electronic");
            var food = categories.FirstOrDefault(c => c.Name == "Food");
            var home = categories.FirstOrDefault(c => c.Name == "Home");
            var toy = categories.FirstOrDefault(c => c.Name == "Toy");

            if (electronic == null || food == null || home == null || toy == null)
            {
                throw new InvalidOperationException("One or more required categories are missing from the database.");
            }

            var products = new List<Product>
            {
                new Product
                {
                    Name = "Electronic Product 1",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                    Price = 499.99m,
                    Stock = 10,
                    CategoryId = electronic.Id,
                    ImageUrl = "/images/electronic1.PNG"
                },
                
                new Product
                {
                    Name = "Electronic Product 2",
                    Description = "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                    Price = 349.99m,
                    Stock = 15,
                    CategoryId = electronic.Id,
                    ImageUrl = "/images/electronic2.PNG"
                },
                
                new Product
                {
                    Name = "Food Product 1",
                    Description = "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
                    Price = 12.99m,
                    Stock = 50,
                    CategoryId = food.Id,
                    ImageUrl = "/images/food1.PNG"
                },
                
                new Product
                {
                    Name = "Food Product 2",
                    Description = "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                    Price = 8.99m,
                    Stock = 30,
                    CategoryId = food.Id,
                    ImageUrl = "/images/food2.PNG"
                },
                
                new Product
                {
                    Name = "Home Product 1",
                    Description = "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium.",
                    Price = 129.99m,
                    Stock = 12,
                    CategoryId = home.Id,
                    ImageUrl = "/images/home1.PNG"
                },
                
                new Product
                {
                    Name = "Home Product 2",
                    Description = "Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores.",
                    Price = 199.99m,
                    Stock = 8,
                    CategoryId = home.Id,
                    ImageUrl = "/images/home2.PNG"
                },
                
                new Product
                {
                    Name = "Toy Product 1",
                    Description = "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit.",
                    Price = 24.99m,
                    Stock = 25,
                    CategoryId = toy.Id,
                    ImageUrl = "/images/toy1.PNG"
                },
                
                new Product
                {
                    Name = "Toy Product 2",
                    Description = "Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur.",
                    Price = 34.99m,
                    Stock = 18,
                    CategoryId = toy.Id,
                    ImageUrl = "/images/toy2.PNG"
                }
            };
            
            if (products.Any())
            {
                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }

        string adminRole = "Admin";
        string adminEmail = "admin@onlinestore.com";
        string adminPassword = "Admin@123";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var user = new IdentityUser 
            { 
                UserName = adminEmail, 
                Email = adminEmail, 
                EmailConfirmed = true 
            };
            
            var createUser = await userManager.CreateAsync(user, adminPassword);
            if (createUser.Succeeded)
            {
                await userManager.AddToRoleAsync(user, adminRole);
            }
        }
        else if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
}
