using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.ScientificDisciplines
{
    public class CreateModel : PageModel
    {
        private readonly IScientificDisciplineService _disciplineService;

        public CreateModel(IScientificDisciplineService disciplineService)
        {
            _disciplineService = disciplineService;
        }

        [BindProperty]
        public ScientificDisciplineInputViewModel DisciplineVM { get; set; } = new ScientificDisciplineInputViewModel();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _disciplineService.CreateDisciplineAsync(DisciplineVM);
            return RedirectToPage("./Index");
        }
    }
}