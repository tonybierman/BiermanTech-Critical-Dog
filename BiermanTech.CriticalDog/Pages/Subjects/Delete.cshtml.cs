using AutoMapper;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Subjects
{
    public class DeleteModel : PageModel
    {
        private readonly ISubjectService _subjectService;
        private readonly IMapper _mapper;

        public DeleteModel(ISubjectService subjectService, IMapper mapper)
        {
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
                await _subjectService.DeleteSubjectAsync(SubjectVM.Id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}