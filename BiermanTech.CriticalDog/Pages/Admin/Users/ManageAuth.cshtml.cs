using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Admin.Users
{
    [Authorize(Policy = "RequireAdminRole")]
    public class ManageAuthModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ManageAuthModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string UserId { get; set; }
            public bool TwoFactorEnabled { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                UserId = id,
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user)
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(Input.UserId);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.SetTwoFactorEnabledAsync(user, Input.TwoFactorEnabled);
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostReset2FAAsync()
        {
            var user = await _userManager.FindByIdAsync(Input.UserId);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            return RedirectToPage("./Index");
        }
    }
}
