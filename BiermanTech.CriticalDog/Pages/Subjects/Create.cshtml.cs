using AutoMapper;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Pages.Subjects
{
    public class CreateModel : PageModel
    {
        private readonly ISubjectService _subjectService;

        public CreateModel(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [BindProperty]
        public SubjectInputViewModel SubjectVM { get; set; } = new SubjectInputViewModel();

        public SelectList SubjectTypes { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
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

            await _subjectService.CreateSubjectAsync(SubjectVM);
            return RedirectToPage("./Index");
        }
    }
}