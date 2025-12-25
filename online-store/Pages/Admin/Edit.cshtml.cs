using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using online_store.Data;
using online_store.Models;

namespace online_store.Pages.Admin
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public List<SelectListItem> Categories { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            Product = product;

            Categories = await _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Categories = await _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToListAsync();

                return Page();
            }

            var productInDb = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == Product.Id);

            if (productInDb == null)
            {
                return NotFound();
            }

            productInDb.Name = Product.Name;
            productInDb.Description = Product.Description;
            productInDb.Price = Product.Price;
            productInDb.Stock = Product.Stock;
            productInDb.CategoryId = Product.CategoryId;
            productInDb.ImageUrl = Product.ImageUrl;

            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
