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
        private readonly ILogger<EditModel> _logger;
        private readonly ISubjectService _subjectService;

        public EditModel(ISubjectService subjectService, ILogger<EditModel> logger)
        {
            _logger = logger;
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
                return this.SetModelStateErrorMessage();
            }

            try
            {
                bool success = await ServiceHelper.ExecuteAsyncOperation(
                    () => _subjectService.UpdateSubjectAsync(SubjectVM),
                    TempData,
                    _logger,
                    successMessage: "Record updated.",
                    failureMessage: "Record not updated."
                );

                if (success)
                {
                    return RedirectToPage("./Index");
                }

                // If not successful, TempData already has the warning message
                SubjectTypes = await _subjectService.GetSubjectTypesSelectListAsync();
                return Page();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}