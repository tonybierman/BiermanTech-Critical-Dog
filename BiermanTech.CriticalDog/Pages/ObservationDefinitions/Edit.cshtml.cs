using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Pages.ObservationDefinitions
{
    public class EditModel : PageModel
    {
        private readonly IObservationDefinitionService _definitionService;

        public EditModel(IObservationDefinitionService definitionService)
        {
            _definitionService = definitionService;
        }

        [BindProperty]
        public ObservationDefinitionInputViewModel DefinitionVM { get; set; } = new ObservationDefinitionInputViewModel();

        public SelectList ObservationTypes { get; set; }
        public SelectList ScientificDisciplines { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            DefinitionVM = await _definitionService.GetDefinitionViewModelByIdAsync(id);
            if (DefinitionVM == null)
            {
                return NotFound();
            }

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

            try
            {
                await _definitionService.UpdateDefinitionAsync(DefinitionVM);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}