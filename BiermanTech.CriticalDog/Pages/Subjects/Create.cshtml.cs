using AutoMapper;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Composition;

namespace BiermanTech.CriticalDog.Pages.Subjects
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _logger;
        private readonly ISubjectService _subjectService;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(ISubjectService subjectService, UserManager<IdentityUser> userManager, ILogger<CreateModel> logger)
        {
            _logger = logger;
            _subjectService = subjectService;
            _userManager = userManager;
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
            // Remove UserId from ModelState validation since it’s set programmatically
            ModelState.Remove("SubjectVM.UserId");

            // Set UserId
            SubjectVM.UserId = _userManager.GetUserId(User);
            if (SubjectVM.UserId == null)
            {
                ModelState.AddModelError(string.Empty, "User not authenticated.");
                SubjectTypes = await _subjectService.GetSubjectTypesSelectListAsync();
                return Page();
            }

            if (!ModelState.IsValid)
            {
                SubjectTypes = await _subjectService.GetSubjectTypesSelectListAsync();
                return Page();
            }

            SubjectVM.UserId = _userManager.GetUserId(User);

            try
            {
                int rows = await _subjectService.CreateSubjectAsync(SubjectVM);

                if (rows > 0)
                {
                    TempData[Constants.AlertSuccess] = $"Record saved.  {rows} rows affected.";
                }
                else
                {
                    TempData[Constants.AlertWarning] = $"Record not saved.  {rows} rows affected.";
                }
            }
            catch (Exception ex)
            {
                TempData[Constants.AlertDanger] = ex.GetAllExceptionMessages();
                _logger.LogError(ex, ex.GetAllExceptionMessages());
            }

            return RedirectToPage("./Index");
        }
    }
}