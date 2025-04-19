using AutoMapper;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Subjects
{
    public class DeleteModel : PageModel
    {
        private readonly ILogger<DeleteModel> _logger;
        private readonly ISubjectService _subjectService;
        private readonly IMapper _mapper;

        public DeleteModel(ISubjectService subjectService, IMapper mapper, ILogger<DeleteModel> logger)
        {
            _logger = logger;
            _subjectService = subjectService;
            _mapper = mapper;
        }

        [BindProperty]
        public SubjectInputViewModel SubjectVM { get; set; } = new SubjectInputViewModel();

        public string SubjectTypeName { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            SubjectVM = _mapper.Map<SubjectInputViewModel>(subject);
            SubjectTypeName = subject.SubjectType?.TypeName ?? "Unknown";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                bool success = await ServiceHelper.ExecuteAsyncOperation(
                    () => _subjectService.DeleteSubjectAsync(SubjectVM.Id),
                    TempData,
                    _logger,
                    successMessage: "Record deleted.",
                    failureMessage: "Record not deleted."
                );

                if (success)
                {
                    return RedirectToPage("./Index");
                }

                // If not successful, TempData already has the warning message
                return Page();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}