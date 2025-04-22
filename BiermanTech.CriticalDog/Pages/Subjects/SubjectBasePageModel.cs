using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Subjects
{
    public abstract class SubjectBasePageModel : PageModel
    {
        protected readonly ISubjectService _subjectService;
        protected readonly IMapper _mapper;
        protected readonly IAuthorizationService _authorizationService;
        protected readonly ILogger _logger;

        protected SubjectBasePageModel(
            ISubjectService subjectService,
            IMapper mapper,
            IAuthorizationService authorizationService,
            ILogger logger)
        {
            _subjectService = subjectService;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        [BindProperty]
        public SubjectInputViewModel SubjectVM { get; set; } = new SubjectInputViewModel();

        public SelectList SubjectTypes { get; set; }

        public string SubjectTypeName { get; set; }

        [BindProperty]
        public bool AnonymousCanView { get; set; }
        [BindProperty]
        public bool AuthenticatedCanView { get; set; }
        [BindProperty]
        public bool AuthenticatedCanEdit { get; set; }
        [BindProperty]
        public bool OwnerCanView { get; set; } = true;
        [BindProperty]
        public bool OwnerCanEdit { get; set; }
        [BindProperty]
        public bool OwnerCanDelete { get; set; }
        [BindProperty]
        public bool AdminCanView { get; set; } = true;
        [BindProperty]
        public bool AdminCanEdit { get; set; } = true;
        [BindProperty]
        public bool AdminCanDelete { get; set; } = true;

        protected async Task<bool> RetrieveAndAuthorizeSubjectAsync(int id, string permission)
        {
            _logger.LogInformation($"RetrieveAndAuthorizeSubjectAsync: Attempting to retrieve Subject with ID {id} for {permission}. User: {User.Identity.Name}, IsAdmin: {User.IsInRole("Admin")}");

            SubjectVM = await _subjectService.GetSubjectViewModelByIdAsync(id);
            if (SubjectVM == null)
            {
                _logger.LogWarning($"RetrieveAndAuthorizeSubjectAsync: Subject with ID {id} not found or user lacks view permissions.");
                return false;
            }

            _logger.LogInformation($"RetrieveAndAuthorizeSubjectAsync: Subject retrieved: ID={SubjectVM.Id}, Permissions={SubjectVM.Permissions}, UserId={SubjectVM.UserId}");

            // Set SubjectTypeName
            var subjectEntity = await _subjectService.GetSubjectByIdAsync(id);
            SubjectTypeName = subjectEntity?.SubjectType?.Name ?? "Unknown";

            // Map SubjectVM to Subject for authorization
            var subject = _mapper.Map<Subject>(SubjectVM);

            // Perform authorization check
            var policyName = $"Subject{permission}";
            var authorizationResult = await _authorizationService.AuthorizeAsync(
                User,
                subject,
                policyName);
            if (!authorizationResult.Succeeded)
            {
                _logger.LogWarning($"RetrieveAndAuthorizeSubjectAsync: Authorization failed for Subject ID {id}. User lacks {permission} permission.");
                return false;
            }

            return true;
        }

        protected void SetPermissionCheckboxes()
        {
            AnonymousCanView = (SubjectVM.Permissions & SubjectPermissions.AnonymousCanView) != 0;
            AuthenticatedCanView = (SubjectVM.Permissions & SubjectPermissions.AuthenticatedCanView) != 0;
            AuthenticatedCanEdit = (SubjectVM.Permissions & SubjectPermissions.AuthenticatedCanEdit) != 0;
            OwnerCanView = true;
            OwnerCanEdit = (SubjectVM.Permissions & SubjectPermissions.OwnerCanEdit) != 0;
            OwnerCanDelete = (SubjectVM.Permissions & SubjectPermissions.OwnerCanDelete) != 0;
            AdminCanView = true;
            AdminCanEdit = true;
            AdminCanDelete = true;
        }

        protected void UpdatePermissionsFromCheckboxes()
        {
            int permissions = 0;

            if (AnonymousCanView)
                permissions |= SubjectPermissions.AnonymousCanView;
            if (AuthenticatedCanView)
                permissions |= SubjectPermissions.AuthenticatedCanView;
            if (AuthenticatedCanEdit)
                permissions |= SubjectPermissions.AuthenticatedCanEdit;
            if (OwnerCanView)
                permissions |= SubjectPermissions.OwnerCanView;
            if (OwnerCanEdit)
                permissions |= SubjectPermissions.OwnerCanEdit;
            if (OwnerCanDelete)
                permissions |= SubjectPermissions.OwnerCanDelete;
            if (AdminCanView)
                permissions |= SubjectPermissions.AdminCanView;
            if (AdminCanEdit)
                permissions |= SubjectPermissions.AdminCanEdit;
            if (AdminCanDelete)
                permissions |= SubjectPermissions.AdminCanDelete;

            SubjectVM.Permissions = permissions;
        }

        protected void EnsureRequiredPermissions()
        {
            SubjectVM.Permissions |= SubjectPermissions.OwnerCanView |
                                     SubjectPermissions.AdminCanView |
                                     SubjectPermissions.AdminCanEdit |
                                     SubjectPermissions.AdminCanDelete;
        }
    }
}