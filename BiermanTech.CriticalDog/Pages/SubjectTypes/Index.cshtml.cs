using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.SubjectTypes
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