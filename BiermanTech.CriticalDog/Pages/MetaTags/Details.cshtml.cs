using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.MetaTags
{
    public class DetailsModel : MetaTagBasePageModel
    {
        public DetailsModel(
            IMetaTagService metaTagService,
            AppDbContext context,
            UserManager<IdentityUser> userManager)
            : base(metaTagService, context, userManager)
        {
        }

        public MetaTagInputViewModel MetaTagVM { get; set; } = new MetaTagInputViewModel();

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

            return Page();
        }
    }
}