using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.MetaTags
{
    [Authorize(Policy = "RequireAuthenticated")]
    public class CreateModel : MetaTagBasePageModel
    {
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            IMetaTagService metaTagService,
            AppDbContext context,
            UserManager<IdentityUser> userManager,
            ILogger<CreateModel> logger)
            : base(metaTagService, context, userManager)
        {
            _logger = logger;
        }

        [BindProperty]
        public MetaTagInputViewModel MetaTagVM { get; set; } = new MetaTagInputViewModel();

        public IList<IdentityUser> Users { get; set; } = new List<IdentityUser>();

        public async Task<IActionResult> OnGetAsync()
        {
            var isAdmin = await IsAdminAsync();
            if (isAdmin)
            {
                Users = await _userManager.Users.ToListAsync();
                MetaTagVM.UserId = null; // Default to system-scoped for admins
            }
            else
            {
                MetaTagVM.UserId = await GetCurrentUserIdAsync(); // Set UserId for non-admins
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var isAdm = await IsAdminAsync();
                if (isAdm)
                {
                    Users = await _userManager.Users.ToListAsync();
                }
                else
                {
                    MetaTagVM.UserId = await GetCurrentUserIdAsync(); // Restore UserId for non-admins
                }
                return Page();
            }

            var isAdmin = await IsAdminAsync();
            if (!isAdmin && MetaTagVM.IsSystemScoped)
            {
                ModelState.AddModelError(string.Empty, "Only administrators can create system-scoped meta tags.");
                if (isAdmin)
                {
                    Users = await _userManager.Users.ToListAsync();
                }
                else
                {
                    MetaTagVM.UserId = await GetCurrentUserIdAsync(); // Restore UserId for non-admins
                }
                return Page();
            }

            await _metaTagService.CreateMetaTagAsync(MetaTagVM);
            return RedirectToPage("./Index");
        }
    }
}