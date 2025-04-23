using AutoMapper;
using BiermanTech.CriticalDog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Subjects
{
    public class EditModel : SubjectBasePageModel
    {
        private readonly ISelectListService _selectListService;
        public SelectList MetaTags { get; set; }

        public EditModel(
            ISubjectService subjectService,
            ISelectListService selectListService,
            IMapper mapper,
            IAuthorizationService authorizationService,
            ILogger<EditModel> logger)
            : base(subjectService, mapper, authorizationService, logger)
        {
            _selectListService = selectListService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!await RetrieveAndAuthorizeSubjectAsync(id, "CanEdit"))
            {
                return Forbid();
            }

            SetPermissionCheckboxes();
            SubjectTypes = await _selectListService.GetSubjectTypesSelectListAsync();
            MetaTags = await _selectListService.GetMetaTagsSelectListAsync();

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
                MetaTags = await _selectListService.GetMetaTagsSelectListAsync();

                return this.SetModelStateErrorMessage();
            }

            // Update SubjectVM.Permissions based on checkbox states
            UpdatePermissionsFromCheckboxes();

            // Ensure required permissions are still set
            EnsureRequiredPermissions();

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

                SubjectTypes = await _selectListService.GetSubjectTypesSelectListAsync();
                MetaTags = await _selectListService.GetMetaTagsSelectListAsync();

                return Page();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to edit subject.");
                TempData["WarningMessage"] = "You are not authorized to edit this subject.";
                SubjectTypes = await _selectListService.GetSubjectTypesSelectListAsync();
                MetaTags = await _selectListService.GetMetaTagsSelectListAsync();

                return Page();
            }
        }
    }
}