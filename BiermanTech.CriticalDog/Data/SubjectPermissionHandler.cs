using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BiermanTech.CriticalDog.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BiermanTech.CriticalDog.Data
{
    public class SubjectPermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public SubjectPermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }

    public class SubjectPermissionHandler : AuthorizationHandler<SubjectPermissionRequirement, Subject>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<SubjectPermissionHandler> _logger;

        public SubjectPermissionHandler(UserManager<IdentityUser> userManager, ILogger<SubjectPermissionHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SubjectPermissionRequirement requirement,
            Subject subject)
        {
            if (subject == null)
            {
                _logger.LogWarning("SubjectPermissionHandler: Subject is null.");
                return;
            }

            var user = context.User;
            bool isAdmin = user.IsInRole("Admin");
            string userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation($"SubjectPermissionHandler: Permission={requirement.Permission}, SubjectId={subject.Id}, Permissions={subject.Permissions}, IsAdmin={isAdmin}, UserId={userId}");

            switch (requirement.Permission)
            {
                case "CanView":
                    if ((subject.Permissions & SubjectPermissions.AnonymousCanView) != 0)
                    {
                        _logger.LogInformation("SubjectPermissionHandler: CanView succeeded due to AnonymousCanView.");
                        context.Succeed(requirement);
                    }
                    if (user.Identity.IsAuthenticated && (subject.Permissions & SubjectPermissions.AuthenticatedCanView) != 0)
                    {
                        _logger.LogInformation("SubjectPermissionHandler: CanView succeeded due to AuthenticatedCanView.");
                        context.Succeed(requirement);
                    }
                    if ((subject.Permissions & SubjectPermissions.OwnerCanView) != 0 && subject.UserId == userId)
                    {
                        _logger.LogInformation("SubjectPermissionHandler: CanView succeeded due to OwnerCanView.");
                        context.Succeed(requirement);
                    }
                    if (isAdmin && (subject.Permissions & SubjectPermissions.AdminCanView) != 0)
                    {
                        _logger.LogInformation("SubjectPermissionHandler: CanView succeeded due to AdminCanView.");
                        context.Succeed(requirement);
                    }
                    break;

                case "CanEdit":
                    if (user.Identity.IsAuthenticated && (subject.Permissions & SubjectPermissions.AuthenticatedCanEdit) != 0)
                    {
                        _logger.LogInformation("SubjectPermissionHandler: CanEdit succeeded due to AuthenticatedCanEdit.");
                        context.Succeed(requirement);
                    }
                    if ((subject.Permissions & SubjectPermissions.OwnerCanEdit) != 0 && subject.UserId == userId)
                    {
                        _logger.LogInformation("SubjectPermissionHandler: CanEdit succeeded due to OwnerCanEdit.");
                        context.Succeed(requirement);
                    }
                    if (isAdmin && (subject.Permissions & SubjectPermissions.AdminCanEdit) != 0)
                    {
                        _logger.LogInformation("SubjectPermissionHandler: CanEdit succeeded due to AdminCanEdit.");
                        context.Succeed(requirement);
                    }
                    else
                    {
                        _logger.LogWarning($"SubjectPermissionHandler: CanEdit failed. IsAdmin={isAdmin}, AdminCanEdit={(subject.Permissions & SubjectPermissions.AdminCanEdit) != 0}");
                    }
                    break;

                case "CanDelete":
                    if ((subject.Permissions & SubjectPermissions.OwnerCanDelete) != 0 && subject.UserId == userId)
                    {
                        _logger.LogInformation("SubjectPermissionHandler: CanDelete succeeded due to OwnerCanDelete.");
                        context.Succeed(requirement);
                    }
                    if (isAdmin && (subject.Permissions & SubjectPermissions.AdminCanDelete) != 0)
                    {
                        _logger.LogInformation("SubjectPermissionHandler: CanDelete succeeded due to AdminCanDelete.");
                        context.Succeed(requirement);
                    }
                    break;
            }
        }
    }
}