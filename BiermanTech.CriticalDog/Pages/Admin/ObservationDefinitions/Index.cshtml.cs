using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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
        public Dictionary<int, string> ScientificDisciplineNames { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, string> MetricTypeNames { get; set; } = new Dictionary<int, string>();

        public async Task OnGetAsync()
        {
            Definitions = await _definitionService.GetAllDefinitionsAsync();

            foreach (var definition in Definitions)
            {
                var entity = await _definitionService.GetDefinitionByIdAsync(definition.Id);
                ObservationTypeNames[definition.Id] = entity.ObservationType?.TypeName ?? "Unknown";
                ScientificDisciplineNames[definition.Id] = entity.ScientificDisciplines.Any()
                    ? string.Join(", ", entity.ScientificDisciplines.Select(sd => sd.DisciplineName))
                    : "None";
                MetricTypeNames[definition.Id] = entity.MetricTypes.Any()
                    ? string.Join(", ", entity.MetricTypes.Select(mt => mt.Description))
                    : "None";
            }
        }

        public async Task<IActionResult> OnGetCloneAsync(int id)
        {
            try
            {
                var definition = await _definitionService.GetDefinitionByIdAsync(id);
                if (definition == null)
                {
                    return NotFound();
                }

                // Create a cloned view model with a unique name
                var clonedDefinition = new ObservationDefinitionInputViewModel
                {
                    DefinitionName = $"{definition.DefinitionName}_Copy_{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Description = definition.Description,
                    MinimumValue = definition.MinimumValue,
                    MaximumValue = definition.MaximumValue,
                    IsActive = definition.IsActive,
                    ObservationTypeId = definition.ObservationTypeId,
                    SelectedScientificDisciplineIds = definition.ScientificDisciplines.Select(a => a.Id).ToList(),
                    MetricTypeIds = definition.MetricTypes.Select(a => a.Id).ToList()
                };

                // Store the cloned view model in TempData
                TempData["ClonedDefinition"] = JsonSerializer.Serialize(clonedDefinition);

                // Redirect to the Create page
                return RedirectToPage("./Create");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while cloning the observation definition. Please try again.";
                // Log the error (assuming ILogger is injected)
                // _logger.LogError(ex, "Error cloning ObservationDefinition with ID {Id}", id);
                return RedirectToPage("./Index");
            }
        }
    }
}