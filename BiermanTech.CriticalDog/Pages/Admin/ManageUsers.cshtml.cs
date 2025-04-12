using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin
{
    public class ManageUsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManageUsersModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public string UserEmail { get; set; }

        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(UserEmail))
            {
                Message = "Please provide an email.";
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(UserEmail);
            if (user == null)
            {
                Message = "User not found.";
                return Page();
            }

            // Ensure Admin role exists
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Add user to Admin role
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var result = await _userManager.AddToRoleAsync(user, "Admin");
                if (result.Succeeded)
                {
                    Message = $"{UserEmail} is now an admin.";
                }
                else
                {
                    Message = "Failed to make user admin: " + string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }
            else
            {
                Message = $"{UserEmail} is already an admin.";
            }

            return Page();
        }
    }
}