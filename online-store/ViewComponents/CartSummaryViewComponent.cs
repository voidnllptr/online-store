using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online_store.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using online_store.ViewModels;

namespace online_store.ViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public CartSummaryViewComponent(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
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
                query = query.Where(ci => ci.SessionId == sessionId);
            }

            var cartItems = await query
                .Include(ci => ci.Product)
                .ToListAsync();

            var itemCount = cartItems.Sum(ci => ci.Quantity);
            var total = cartItems.Sum(ci => ci.Quantity * ci.Product.Price);

            return View(new CartSummaryViewModel
            {
                ItemCount = itemCount,
                Total = total
            });
        }
    }
}
