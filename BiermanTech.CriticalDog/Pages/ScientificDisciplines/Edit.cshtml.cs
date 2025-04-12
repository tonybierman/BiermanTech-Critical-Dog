using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.ScientificDisciplines
{
    public class EditModel : PageModel
    {
        private readonly IScientificDisciplineService _disciplineService;

        public EditModel(IScientificDisciplineService disciplineService)
        {
            _disciplineService = disciplineService;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _disciplineService.UpdateDisciplineAsync(DisciplineVM);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}