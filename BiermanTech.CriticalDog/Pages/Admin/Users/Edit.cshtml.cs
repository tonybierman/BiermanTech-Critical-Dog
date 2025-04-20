using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BiermanTech.CriticalDog.Pages.Admin.Users
{
    [Authorize(Policy = "RequireAdminRole")]
    public class EditUserModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditUserModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Id { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            public bool IsAdmin { get; set; }
            public bool CanView { get; set; }
            public bool CanEdit { get; set; }
            public bool CanDelete { get; set; }
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
                Id = user.Id,
                Email = user.Email,
                IsAdmin = (await _userManager.GetRolesAsync(user)).Contains("Admin"),
                CanView = (await _userManager.GetClaimsAsync(user)).Any(c => c.Type == "SubjectPermission" && c.Value == "CanView"),
                CanEdit = (await _userManager.GetClaimsAsync(user)).Any(c => c.Type == "SubjectPermission" && c.Value == "CanEdit"),
                CanDelete = (await _userManager.GetClaimsAsync(user)).Any(c => c.Type == "SubjectPermission" && c.Value == "CanDelete")
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(Input.Id);
            if (user == null)
            {
                return NotFound();
            }

            // Update roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (Input.IsAdmin && !currentRoles.Contains("Admin"))
            {
                // Ensure the Admin role exists
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else if (!Input.IsAdmin && currentRoles.Contains("Admin"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }

            // Update claims
            var currentClaims = await _userManager.GetClaimsAsync(user);
            var newClaims = new List<Claim>();

            if (Input.CanView)
                newClaims.Add(new Claim("SubjectPermission", "CanView"));
            if (Input.CanEdit)
                newClaims.Add(new Claim("SubjectPermission", "CanEdit"));
            if (Input.CanDelete)
                newClaims.Add(new Claim("SubjectPermission", "CanDelete"));

            await _userManager.RemoveClaimsAsync(user, currentClaims.Where(c => c.Type == "SubjectPermission"));
            await _userManager.AddClaimsAsync(user, newClaims);

            return RedirectToPage("./Index");
        }
    }
}
