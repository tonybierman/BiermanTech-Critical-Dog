using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.MetaTags
{
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

        public async Task<IActionResult> OnGetAsync()
        {
            MetaTagVM.IsSystemScoped = false; // Default to user-scoped
            MetaTagVM.UserId = await GetCurrentUserIdAsync(); // Set UserId for form
            if (!await IsAdminAsync())
            {
                MetaTagVM.IsSystemScoped = false; // Non-admins cannot create system-scoped tags
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                MetaTagVM.UserId = await GetCurrentUserIdAsync(); // Restore UserId if validation fails
                return Page();
            }

            var isAdmin = await IsAdminAsync();
            if (!isAdmin && MetaTagVM.IsSystemScoped)
            {
                ModelState.AddModelError(string.Empty, "Only administrators can create system-scoped meta tags.");
                MetaTagVM.UserId = await GetCurrentUserIdAsync(); // Restore UserId
                return Page();
            }

            await _metaTagService.CreateMetaTagAsync(MetaTagVM);
            return RedirectToPage("./Index");
        }
    }
}