using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Pages.Shared;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace BiermanTech.CriticalDog.Pages.Admin.ObservationDefinitions
{
    public class CreateModel : PageModel
    {
        private readonly ISelectListService _selectListService;
        private readonly IObservationDefinitionService _definitionService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IObservationDefinitionService definitionService, ISelectListService selectListService, ILogger<CreateModel> logger)
        {
            _selectListService = selectListService;
            _definitionService = definitionService;
            _logger = logger;
        }

        [BindProperty]
        public ObservationDefinitionInputViewModel DefinitionVM { get; set; } = new ObservationDefinitionInputViewModel();

        public SelectList ObservationTypes { get; set; }
        public SelectList ScientificDisciplines { get; set; }
        public SelectList MetricTypes { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Load select lists
            await EnsureSelectLists();

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
                await EnsureSelectLists();
                return Page();
            }

            try
            {
                await _definitionService.CreateDefinitionAsync(DefinitionVM);
                TempData[Constants.AlertSuccess] = "Observation definition created successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                TempData[Constants.AlertDanger] = $"An error occurred while creating the observation definition. {ex.GetAllExceptionMessages()}";
                _logger.LogError(ex, "Error creating ObservationDefinition.");
                await EnsureSelectLists();
                return Page();
            }
        }

        private async Task EnsureSelectLists()
        {
            ObservationTypes = await _selectListService.GetObservationTypesSelectListAsync();
            ScientificDisciplines = await _selectListService.GetScientificDisciplinesSelectListAsync();
            MetricTypes = await _selectListService.GetMetricTypesSelectListAsync();
        }
    }
}