using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using online_store.Data;
using online_store.Models;

namespace online_store.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context; 
        }

        public IList<Product> Products { get; set; } = new List<Product>();

        public async Task OnGetAsync()
        {
            Products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }
    }
}
