using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Pages.Admin.ObservationDefinitions
{
    public class CreateModel : PageModel
    {
        private readonly IObservationDefinitionService _definitionService;

        public CreateModel(IObservationDefinitionService definitionService)
        {
            _definitionService = definitionService;
        }

        [BindProperty]
        public ObservationDefinitionInputViewModel DefinitionVM { get; set; } = new ObservationDefinitionInputViewModel();

        public SelectList ObservationTypes { get; set; }
        public SelectList ScientificDisciplines { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ObservationTypes = await _definitionService.GetObservationTypesSelectListAsync();
            ScientificDisciplines = await _definitionService.GetScientificDisciplinesSelectListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ObservationTypes = await _definitionService.GetObservationTypesSelectListAsync();
                ScientificDisciplines = await _definitionService.GetScientificDisciplinesSelectListAsync();
                return Page();
            }

            await _definitionService.CreateDefinitionAsync(DefinitionVM);
            return RedirectToPage("./Index");
        }
    }
}