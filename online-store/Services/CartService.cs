using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using online_store.Models;

namespace online_store.Services
{
    public class CartService
    {
        private readonly Data.ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public CartService(
            Data.ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task AddToCartAsync(int productId, int quantity = 1)
        {
            try
            {
                _httpContextAccessor.HttpContext.Session.SetString("CartInitialized", "true");
                var sessionId = _httpContextAccessor.HttpContext.Session.Id;
                
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    throw new ArgumentException("Товар не найден", nameof(productId));
                }

                if (quantity <= 0)
                {
                    throw new ArgumentException("Количество должно быть больше нуля", nameof(quantity));
                }

                var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
                
                var query = _context.CartItems.AsQueryable();
                if (userId != null)
                {
                    query = query.Where(ci => ci.UserId == userId);
                }
                else
                {
                    query = query.Where(ci => ci.SessionId == sessionId);
                }
                
                var cartItem = await query.FirstOrDefaultAsync(ci => ci.ProductId == productId);

                if (cartItem != null)
                {
                    cartItem.Quantity = Math.Min(cartItem.Quantity + quantity, product.Stock);
                }
                else
                {
                    cartItem = new CartItem
                    {
                        ProductId = productId,
                        Quantity = Math.Min(quantity, product.Stock),
                        UserId = userId,
                        SessionId = userId == null ? sessionId : null, // Only store session ID for anonymous users
                        DateCreated = DateTime.UtcNow
                    };
                    _context.CartItems.Add(cartItem);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddToCartAsync: {ex}");
                throw;
            }
        }

        public async Task<int> GetCartItemCountAsync()
        {
            try
            {
                var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
                var sessionId = _httpContextAccessor.HttpContext.Session.Id;

                var query = _context.CartItems.AsQueryable();

                if (userId != null)
                {
                    query = query.Where(ci => ci.UserId == userId);
                }
                else
                {
                    if (!string.IsNullOrEmpty(sessionId))
                    {
                        query = query.Where(ci => ci.SessionId == sessionId);
                    }
                    else
                    {
                        return 0;
                    }
                }

                return await query.SumAsync(ci => ci.Quantity);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetCartItemCountAsync: {ex}");
                return 0;
            }
        }

        public async Task MergeCartsOnLoginAsync(string userId)
        {
            try
            {
                var sessionId = _httpContextAccessor.HttpContext.Session.Id;
                if (string.IsNullOrEmpty(sessionId))
                    return;
                
                var anonymousCartItems = await _context.CartItems
                    .Include(ci => ci.Product)
                    .Where(ci => ci.SessionId == sessionId && ci.UserId == null)
                    .ToListAsync();

                if (!anonymousCartItems.Any())
                    return;

                var userCartItems = await _context.CartItems
                    .Include(ci => ci.Product)
                    .Where(ci => ci.UserId == userId)
                    .ToListAsync();

                foreach (var item in anonymousCartItems)
                {
                    var existingItem = userCartItems.FirstOrDefault(ci => ci.ProductId == item.ProductId);
                    
                    if (existingItem != null)
                    {
                        int maxQuantity = existingItem.Product?.Stock ?? int.MaxValue;
                        existingItem.Quantity = Math.Min(existingItem.Quantity + item.Quantity, maxQuantity);
                        _context.CartItems.Remove(item);
                    }
                    else
                    {
                        item.UserId = userId;
                        item.SessionId = null;
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in MergeCartsOnLoginAsync: {ex}");
                throw;
            }
        }
    }
}
