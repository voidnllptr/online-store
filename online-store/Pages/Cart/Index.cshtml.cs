using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using online_store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace online_store.Pages.Cart
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly online_store.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(online_store.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal Total { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await LoadCartItemsAsync();
                return Page();
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Error in Cart/Index OnGetAsync: {ex}");
                // Return the page with an empty cart instead of throwing
                CartItems = new List<CartItem>();
                Total = 0;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateAsync(int id, int quantity)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.Id == id);

            if (cartItem != null)
            {
                if (quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                }
                else
                {
                    // Ensure we don't exceed available stock
                    quantity = Math.Min(quantity, cartItem.Product.Stock);
                    cartItem.Quantity = quantity;
                }
                
                await _context.SaveChangesAsync();
            }

            await LoadCartItemsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            await LoadCartItemsAsync();
            return Page();
        }

        private async Task LoadCartItemsAsync()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var sessionId = HttpContext.Session.Id;

                var query = _context.CartItems
                    .Include(ci => ci.Product)
                    .ThenInclude(p => p.Category)
                    .AsQueryable();

                if (User.Identity.IsAuthenticated && !string.IsNullOrEmpty(userId))
                {
                    query = query.Where(ci => ci.UserId == userId);
                }
                else if (!string.IsNullOrEmpty(sessionId))
                {
                    query = query.Where(ci => ci.SessionId == sessionId);
                }
                else
                {
                    // No valid user or session ID, set empty cart
                    CartItems = new List<CartItem>();
                    Total = 0;
                    return;
                }

                var items = await query.ToListAsync();
                
                // Filter out any items with null products (in case products were deleted)
                items = items.Where(ci => ci.Product != null).ToList();
                
                CartItems = items;
                Total = items.Sum(item => item.Quantity * item.Product.Price);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadCartItemsAsync: {ex}");
                CartItems = new List<CartItem>();
                Total = 0;
            }
        }
    }
}
