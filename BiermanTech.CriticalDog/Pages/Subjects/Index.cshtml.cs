using BiermanTech.CriticalDog.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Pages.Dogs
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(AppDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Subject> Dogs { get; set; } = new List<Subject>();

        public async Task OnGetAsync()
        {
            Dogs = await _context.GetFilteredSubjects().ToListAsync();
        }
    }
}