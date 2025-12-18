using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using online_store.Data;
using online_store.Models;
using online_store.Services;

namespace online_store
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<IdentityUser>(options => 
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/Admin");
                options.Conventions.AllowAnonymousToPage("/Index");
                options.Conventions.AllowAnonymousToPage("/Catalog/Index");
            });

            builder.Services.AddHttpContextAccessor();
            
            builder.Services.AddDistributedMemoryCache();
            
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = ".OnlineStore.Session";
            });
            
            builder.Services.AddScoped<CartService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                await SeedData.Initialize(app.Services);
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseAuthentication();
            
            app.UseSession();
            
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
