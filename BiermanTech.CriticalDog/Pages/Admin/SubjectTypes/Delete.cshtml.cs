using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.SubjectTypes
{
    public class DeleteModel : PageModel
    {
        private readonly ISubjectTypeService _subjectTypeService;

        public DeleteModel(ISubjectTypeService subjectTypeService)
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
            try
            {
                await _subjectTypeService.DeleteSubjectTypeAsync(SubjectTypeVM.Id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}