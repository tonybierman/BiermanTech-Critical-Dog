using AutoMapper;
using BiermanTech.CriticalDog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Subjects
{
    public class DeleteModel : SubjectBasePageModel
    {
        public DeleteModel(ISubjectService subjectService, IMapper mapper, IAuthorizationService authorizationService, ILogger<DeleteModel> logger) :
            base(subjectService, mapper, authorizationService, logger)
        { }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!await RetrieveAndAuthorizeSubjectAsync(id, "CanDelete"))
            {
                return Forbid();
            }

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

                return Page();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to delete subject.");
                TempData["WarningMessage"] = "You are not authorized to delete this subject.";
                return Page();
            }
        }
    }
}