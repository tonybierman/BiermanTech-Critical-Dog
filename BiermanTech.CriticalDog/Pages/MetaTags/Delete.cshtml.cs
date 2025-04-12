using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.MetaTags
{
    public class DeleteModel : PageModel
    {
        private readonly IMetaTagService _metaTagService;

        public DeleteModel(IMetaTagService metaTagService)
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
            try
            {
                await _metaTagService.DeleteMetaTagAsync(MetaTagVM.Id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}