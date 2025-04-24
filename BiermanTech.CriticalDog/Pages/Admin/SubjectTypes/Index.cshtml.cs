using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.SubjectTypes
{
    public class IndexModel : PageModel
    {
        private readonly ISubjectTypeService _subjectTypeService;

        public IndexModel(ISubjectTypeService subjectTypeService)
        {
            _subjectTypeService = subjectTypeService;
        }

        public List<SubjectTypeInputViewModel> SubjectTypes { get; set; } = new List<SubjectTypeInputViewModel>();

        public async Task OnGetAsync()
        {
            SubjectTypes = await _subjectTypeService.GetAllSubjectTypesAsync();
        }
    }
}