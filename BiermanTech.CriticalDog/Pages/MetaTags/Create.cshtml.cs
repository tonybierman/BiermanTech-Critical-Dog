using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.MetaTags
{
    public class CreateModel : PageModel
    {
        private readonly IMetaTagService _metaTagService;

        public CreateModel(IMetaTagService metaTagService)
        {
            _metaTagService = metaTagService;
        }

        [BindProperty]
        public MetaTagInputViewModel MetaTagVM { get; set; } = new MetaTagInputViewModel();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _metaTagService.CreateMetaTagAsync(MetaTagVM);
            return RedirectToPage("./Index");
        }
    }
}