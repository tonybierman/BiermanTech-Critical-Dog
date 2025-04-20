using AutoMapper;
using BiermanTech.CriticalDog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Subjects
{
    public class DetailsModel : SubjectBasePageModel
    {
        public DetailsModel(
            ISubjectService subjectService,
            IMapper mapper,
            IAuthorizationService authorizationService,
            ILogger<DetailsModel> logger)
            : base(subjectService, mapper, authorizationService, logger)
        {
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!await RetrieveAndAuthorizeSubjectAsync(id, "CanView"))
            {
                return NotFound();
            }

            return Page();
        }
    }
}