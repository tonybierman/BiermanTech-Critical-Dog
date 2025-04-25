using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.MetaTags
{
    public class EditModel : MetaTagBasePageModel
    {
        public EditModel(
            IMetaTagService metaTagService,
            AppDbContext context,
            UserManager<IdentityUser> userManager)
            : base(metaTagService, context, userManager)
        {
        }

        [BindProperty]
        public MetaTagInputViewModel MetaTagVM { get; set; } = new MetaTagInputViewModel();

        public IList<IdentityUser> Users { get; set; } = new List<IdentityUser>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!await CanAccessMetaTagAsync(id))
            {
                return NotFound();
            }

            MetaTagVM = await _metaTagService.GetMetaTagViewModelByIdAsync(id);
            if (MetaTagVM == null)
            {
                return NotFound();
            }

            var isAdmin = await IsAdminAsync();
            if (!isAdmin && MetaTagVM.IsSystemScoped)
            {
                ModelState.AddModelError(string.Empty, "Only administrators can edit system-scoped meta tags.");
                return Page();
            }

            if (isAdmin)
            {
                Users = await _userManager.Users.ToListAsync();
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
                return Page();
            }

            var isAdmin = await IsAdminAsync();
            if (!isAdmin && MetaTagVM.IsSystemScoped)
            {
                ModelState.AddModelError(string.Empty, "Only administrators can set meta tags as system-scoped.");
                var isAdminReload = await IsAdminAsync();
                if (isAdminReload)
                {
                    Users = await _userManager.Users.ToListAsync();
                }
                return Page();
            }

            if (!await CanAccessMetaTagAsync(MetaTagVM.Id))
            {
                return NotFound();
            }

            try
            {
                await _metaTagService.UpdateMetaTagAsync(MetaTagVM);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}