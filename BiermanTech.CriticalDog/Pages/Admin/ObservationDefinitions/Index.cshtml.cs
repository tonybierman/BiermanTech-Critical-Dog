using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.ObservationDefinitions
{
    public class IndexModel : PageModel
    {
        private readonly IObservationDefinitionService _definitionService;

        public IndexModel(IObservationDefinitionService definitionService)
        {
            _definitionService = definitionService;
        }

        public List<ObservationDefinitionInputViewModel> Definitions { get; set; } = new List<ObservationDefinitionInputViewModel>();
        public Dictionary<int, string> ObservationTypeNames { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, List<string>> ScientificDisciplineNames { get; set; } = new Dictionary<int, List<string>>();

        public async Task OnGetAsync()
        {
            var definitions = await _definitionService.GetAllDefinitionsAsync();
            Definitions = definitions;

            foreach (var definition in definitions)
            {
                var entity = await _definitionService.GetDefinitionByIdAsync(definition.Id);
                ObservationTypeNames[definition.Id] = entity.ObservationType?.TypeName ?? "Unknown";
                ScientificDisciplineNames[definition.Id] = entity.ScientificDisciplines
                    .Select(d => d.DisciplineName)
                    .ToList();
            }
        }

        public async Task<IActionResult> OnGetCloneAsync(int id)
        {
            var definition = await _definitionService.GetDefinitionByIdAsync(id);
            if (definition == null)
            {
                return NotFound();
            }

            // Create a new definition by copying the existing one
            var clonedDefinition = new ObservationDefinitionInputViewModel
            {
                DefinitionName = definition.DefinitionName + "Copy",
                IsQualitative = definition.IsQualitative,
                MinimumValue = definition.MinimumValue,
                MaximumValue = definition.MaximumValue,
                IsActive = definition.IsActive,
                ObservationTypeId = definition.ObservationTypeId,
                SelectedScientificDisciplineIds = definition.ScientificDisciplines.Select(a => a.Id).ToList()
            };

            // Save the cloned definition
            await _definitionService.CreateDefinitionAsync(clonedDefinition);

            // Redirect to the index page to refresh the list
            return RedirectToPage("./Index");
        }
    }
}