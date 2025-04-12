using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.SubjectTypes
{
    public class CreateModel : PageModel
    {
        private readonly ISubjectTypeService _subjectTypeService;

        public CreateModel(ISubjectTypeService subjectTypeService)
        {
            _subjectTypeService = subjectTypeService;
        }

        [BindProperty]
        public SubjectTypeInputViewModel SubjectTypeVM { get; set; } = new SubjectTypeInputViewModel();

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

            await _subjectTypeService.CreateSubjectTypeAsync(SubjectTypeVM);
            return RedirectToPage("./Index");
        }
    }
}