using AutoMapper;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Pages.Subjects
{
    public class EditModel : PageModel
    {
        private readonly ISubjectService _subjectService;

        public EditModel(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [BindProperty]
        public SubjectInputViewModel SubjectVM { get; set; } = new SubjectInputViewModel();

        public SelectList SubjectTypes { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            SubjectVM = await _subjectService.GetSubjectViewModelByIdAsync(id);
            if (SubjectVM == null)
            {
                return NotFound();
            }

            SubjectTypes = await _subjectService.GetSubjectTypesSelectListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                SubjectTypes = await _subjectService.GetSubjectTypesSelectListAsync();
                return Page();
            }

            try
            {
                await _subjectService.UpdateSubjectAsync(SubjectVM);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}