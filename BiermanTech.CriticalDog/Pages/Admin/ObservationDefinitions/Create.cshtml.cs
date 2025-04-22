using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

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
        public SelectList MetricTypes { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Load select lists
            ObservationTypes = await _definitionService.GetObservationTypesSelectListAsync();
            ScientificDisciplines = await _definitionService.GetScientificDisciplinesSelectListAsync();
            MetricTypes = await _definitionService.GetMetricTypesSelectListAsync();

            // Check for cloned definition in TempData
            if (TempData["ClonedDefinition"] is string clonedJson)
            {
                DefinitionVM = JsonSerializer.Deserialize<ObservationDefinitionInputViewModel>(clonedJson);
                TempData["SuccessMessage"] = "Definition cloned. Please review and save.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ObservationTypes = await _definitionService.GetObservationTypesSelectListAsync();
                ScientificDisciplines = await _definitionService.GetScientificDisciplinesSelectListAsync();
                MetricTypes = await _definitionService.GetMetricTypesSelectListAsync();
                return Page();
            }

            try
            {
                await _definitionService.CreateDefinitionAsync(DefinitionVM);
                TempData["SuccessMessage"] = "Observation definition created successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the observation definition. Please try again.";
                // Log the error (assuming ILogger is injected)
                // _logger.LogError(ex, "Error creating ObservationDefinition");
                ObservationTypes = await _definitionService.GetObservationTypesSelectListAsync();
                ScientificDisciplines = await _definitionService.GetScientificDisciplinesSelectListAsync();
                MetricTypes = await _definitionService.GetMetricTypesSelectListAsync();
                return Page();
            }
        }
    }
}