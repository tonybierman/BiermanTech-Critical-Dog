using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.MetaTags
{
    public class DetailsModel : PageModel
    {
        private readonly IMetaTagService _metaTagService;

        public DetailsModel(IMetaTagService metaTagService)
        {
            _metaTagService = metaTagService;
        }

        public MetaTagInputViewModel MetaTagVM { get; set; } = new MetaTagInputViewModel();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            MetaTagVM = await _metaTagService.GetMetaTagViewModelByIdAsync(id);
            if (MetaTagVM == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}