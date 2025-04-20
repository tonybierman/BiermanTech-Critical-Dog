using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Pages.Admin.Users
{
    [Authorize(Policy = "RequireAdminRole")]
    public class UsersIndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UsersIndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IList<IdentityUser> Users { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _userManager.Users.ToListAsync();
        }
    }
}
