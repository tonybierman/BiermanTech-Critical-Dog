using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.MetaTags
{
    public abstract class MetaTagBasePageModel : PageModel
    {
        protected readonly IMetaTagService _metaTagService;
        protected readonly AppDbContext _context;
        protected readonly UserManager<IdentityUser> _userManager;

        protected MetaTagBasePageModel(
            IMetaTagService metaTagService,
            AppDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _metaTagService = metaTagService;
            _context = context;
            _userManager = userManager;
        }

        protected async Task<string> GetCurrentUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.Id;
        }

        protected async Task<bool> IsAdminAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user != null && await _userManager.IsInRoleAsync(user, "Admin");
        }

        protected async Task<bool> CanAccessMetaTagAsync(int metaTagId)
        {
            var userId = await GetCurrentUserIdAsync();
            if (string.IsNullOrEmpty(userId))
                return false;

            var isAdmin = await IsAdminAsync();
            if (isAdmin)
                return true;

            var metaTag = await _context.MetaTags.FindAsync(metaTagId);
            return metaTag != null && (metaTag.UserId == userId || metaTag.UserId == null);
        }
    }
}