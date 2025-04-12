using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.SubjectTypes
{
    public class EditModel : PageModel
    {
        private readonly ISubjectTypeService _subjectTypeService;

        public EditModel(ISubjectTypeService subjectTypeService)
        {
            _subjectTypeService = subjectTypeService;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _subjectTypeService.UpdateSubjectTypeAsync(SubjectTypeVM);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}