using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using online_store.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace online_store.Pages.Catalog
{
    public class IndexModel : PageModel
    {
        private readonly online_store.Data.ApplicationDbContext _context;
        private const int PageSize = 9;

        public IndexModel(online_store.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Product> Products { get; set; } = new List<Product>();
        public IList<Category> Categories { get; set; } = new List<Category>();
        public List<int>? SelectedCategoryIds { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public async Task<IActionResult> OnGetAsync(int? page, List<int>? categoryIds, string search)
        {
            CurrentPage = page ?? 1;
            SelectedCategoryIds = categoryIds;
            ViewData["SearchString"] = search;

            // Get all categories for the filter
            Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

            // Build the base query
            IQueryable<Product> productsQuery = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // Apply search filter if search term is provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLower().Trim();
                productsQuery = productsQuery.Where(p => p.Name.ToLower().Contains(searchTerm));
            }

            // Apply category filter if any category is selected
            if (SelectedCategoryIds != null && SelectedCategoryIds.Any())
            {
                productsQuery = productsQuery.Where(p => SelectedCategoryIds.Contains(p.CategoryId));
            }

            // Get total count for pagination
            TotalItems = await productsQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            // Apply pagination
            Products = await productsQuery
                .OrderBy(p => p.Name)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }
    }
}
