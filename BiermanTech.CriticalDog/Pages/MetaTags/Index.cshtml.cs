using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.EntityServices;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.MetaTags
{
    public class IndexModel : MetaTagBasePageModel
    {
        public IndexModel(
            IMetaTagService metaTagService,
            AppDbContext context,
            UserManager<IdentityUser> userManager)
            : base(metaTagService, context, userManager)
        {
        }

        public List<MetaTagInputViewModel> MetaTags { get; set; } = new List<MetaTagInputViewModel>();

        public async Task OnGetAsync()
        {
            MetaTags = await _metaTagService.GetAllMetaTagsAsync();
        }
    }
}