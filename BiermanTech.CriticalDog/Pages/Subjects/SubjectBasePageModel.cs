using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Interfaces;
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

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public SubjectInputViewModel SubjectVM { get; set; } = new SubjectInputViewModel();
        public SelectList SubjectTypes { get; set; }
        public string SubjectTypeName { get; set; }
        public List<string> MetaTagNames { get; set; } = new List<string>();

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

        // TODO: Wasted trips to DB
        protected async Task<bool> RetrieveAndAuthorizeSubjectAsync(string permission)
        {
            var entity = await _subjectService.GetSubjectByIdAsync(this.Id);
            if (entity == null)
            {
                _logger.LogWarning($"RetrieveAndAuthorizeSubjectAsync: Subject with ID {this.Id} not found or user lacks view permissions.");
                return false;
            }

            SubjectVM = await _subjectService.GetSubjectViewModelByIdAsync(this.Id);
            SubjectTypeName = entity?.SubjectType?.Name ?? "Unknown";
            MetaTagNames = entity?.MetaTags?.Select(m => m.Name)?.ToList();

            // Map SubjectVM to tmp for authorization check
            var tmp = _mapper.Map<Subject>(SubjectVM);
            var policyName = $"Subject{permission}";
            var authorizationResult = await _authorizationService.AuthorizeAsync(
                User,
                tmp,
                policyName);
            if (!authorizationResult.Succeeded)
            {
                _logger.LogWarning($"RetrieveAndAuthorizeSubjectAsync: Authorization failed for Subject ID {this.Id}. User lacks {permission} permission.");
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