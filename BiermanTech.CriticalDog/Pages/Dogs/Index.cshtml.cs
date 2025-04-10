using BiermanTech.CriticalDog.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Pages.Dogs
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Dog> Dogs { get; set; } = new List<Dog>();

        public async Task OnGetAsync()
        {
            Dogs = await _context.Dogs.ToListAsync();
        }
    }
}