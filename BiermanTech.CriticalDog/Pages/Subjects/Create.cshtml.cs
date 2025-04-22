using AutoMapper;
using BiermanTech.CriticalDog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Subjects
{
    [Authorize(Policy = "RequireAuthenticated")]
    public class CreateModel : SubjectBasePageModel
    {
        private readonly ISelectListService _selectListService;

        public CreateModel(ISubjectService subjectService, ISelectListService selectListService, IMapper mapper, IAuthorizationService authorizationService, ILogger<CreateModel> logger) :
            base(subjectService, mapper, authorizationService, logger) 
        {
            _selectListService = selectListService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            SubjectTypes = await _selectListService.GetSubjectTypesSelectListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("SubjectVM.UserId");
            ModelState.Remove("AnonymousCanView");
            ModelState.Remove("AuthenticatedCanView");
            ModelState.Remove("AuthenticatedCanEdit");
            ModelState.Remove("OwnerCanView");
            ModelState.Remove("OwnerCanEdit");
            ModelState.Remove("OwnerCanDelete");
            ModelState.Remove("AdminCanView");
            ModelState.Remove("AdminCanEdit");
            ModelState.Remove("AdminCanDelete");

            if (!ModelState.IsValid)
            {
                SubjectTypes = await _selectListService.GetSubjectTypesSelectListAsync();
                return this.SetModelStateErrorMessage();
            }

            // Update SubjectVM.Permissions based on checkbox states
            UpdatePermissionsFromCheckboxes();

            // AppDbContext will set UserId and default permissions, but we can ensure required permissions here as well
            EnsureRequiredPermissions();

            try
            {
                bool success = await ServiceHelper.ExecuteAsyncOperation(
                    () => _subjectService.CreateSubjectAsync(SubjectVM),
                    TempData,
                    _logger,
                    successMessage: "Record created.",
                    failureMessage: "Record not created."
                );

                if (success)
                {
                    return RedirectToPage("./Index");
                }

                SubjectTypes = await _selectListService.GetSubjectTypesSelectListAsync();
                return Page();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to create subject.");
                TempData["WarningMessage"] = "You are not authorized to create a subject.";
                SubjectTypes = await _selectListService.GetSubjectTypesSelectListAsync();
                return Page();
            }
        }
    }

}