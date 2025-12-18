using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using online_store.Data;
using online_store.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace online_store.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IList<Product> FeaturedProducts { get; set; } = new List<Product>();

        public async Task<IActionResult> OnGetAsync()
        {
            FeaturedProducts = await _context.Products
                .Where(p => p.Stock > 0)
                .OrderByDescending(p => p.Id)
                .Take(8)
                .ToListAsync();

            return Page();
        }
    }
}
