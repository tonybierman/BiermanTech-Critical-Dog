using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.SubjectTypes
{
    public class DetailsModel : PageModel
    {
        private readonly ISubjectTypeService _subjectTypeService;

        public DetailsModel(ISubjectTypeService subjectTypeService)
        {
            _subjectTypeService = subjectTypeService;
        }

        public SubjectTypeInputViewModel SubjectTypeVM { get; set; } = new SubjectTypeInputViewModel();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            SubjectTypeVM = await _subjectTypeService.GetSubjectTypeViewModelByIdAsync(id);
            if (SubjectTypeVM == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}