using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.ScientificDisciplines
{
    public class DetailsModel : PageModel
    {
        private readonly IScientificDisciplineService _disciplineService;

        public DetailsModel(IScientificDisciplineService disciplineService)
        {
            _disciplineService = disciplineService;
        }

        public ScientificDisciplineInputViewModel DisciplineVM { get; set; } = new ScientificDisciplineInputViewModel();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            DisciplineVM = await _disciplineService.GetDisciplineViewModelByIdAsync(id);
            if (DisciplineVM == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}