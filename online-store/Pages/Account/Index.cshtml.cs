using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using online_store.Models;
using System.Threading.Tasks;

namespace online_store.Pages.Account
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Username = user.UserName;
            Email = user.Email;
            PhoneNumber = user.PhoneNumber;

            return Page();
        }
    }
}
