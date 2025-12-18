using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using online_store.Models;
using online_store.Services;
using Microsoft.AspNetCore.Authorization;

namespace online_store.Pages.Cart
{
    [AllowAnonymous]
    public class AddModel : PageModel
    {
        private readonly online_store.Data.ApplicationDbContext _context;
        private readonly CartService _cartService;

        public AddModel(online_store.Data.ApplicationDbContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = "/";

        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == Id);

            if (Product == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int quantity)
        {
            var product = await _context.Products.FindAsync(Id);
            if (product == null)
            {
                return NotFound();
            }

            if (quantity <= 0 || quantity > product.Stock)
            {
                ModelState.AddModelError(string.Empty, "Некорректное количество товара");
                return await OnGetAsync();
            }

            try
            {
                await _cartService.AddToCartAsync(Id, quantity);
                TempData["SuccessMessage"] = $"{product.Name} добавлен в корзину";
                
                // If the user came from the product page, redirect them back there
                if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Contains("/Catalog/Details/"))
                {
                    return Redirect(ReturnUrl);
                }
                
                return RedirectToPage("/Cart/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return await OnGetAsync();
            }
        }
    }
}
