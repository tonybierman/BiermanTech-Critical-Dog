using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.MetaTags
{
    public class EditModel : PageModel
    {
        private readonly IMetaTagService _metaTagService;

        public EditModel(IMetaTagService metaTagService)
        {
            _metaTagService = metaTagService;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
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