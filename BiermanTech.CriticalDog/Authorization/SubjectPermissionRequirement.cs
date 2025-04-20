using Microsoft.AspNetCore.Authorization;

namespace BiermanTech.CriticalDog.Authorization
{
    public class SubjectPermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public SubjectPermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
