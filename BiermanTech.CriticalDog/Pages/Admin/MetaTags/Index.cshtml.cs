using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.MetaTags
{
    public class IndexModel : PageModel
    {
        private readonly IMetaTagService _metaTagService;

        public IndexModel(IMetaTagService metaTagService)
        {
            _metaTagService = metaTagService;
        }

        public List<MetaTagInputViewModel> MetaTags { get; set; } = new List<MetaTagInputViewModel>();

        public async Task OnGetAsync()
        {
            MetaTags = await _metaTagService.GetAllMetaTagsAsync();
        }
    }
}