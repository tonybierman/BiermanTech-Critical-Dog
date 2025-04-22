using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Pages.Admin.ObservationDefinitions
{
    public class EditModel : PageModel
    {
        private readonly IObservationDefinitionService _definitionService;
        private readonly ISelectListService _selectListService;

        public EditModel(IObservationDefinitionService definitionService, ISelectListService selectListService)
        {
            _definitionService = definitionService;
            _selectListService = selectListService;
        }

        [BindProperty]
        public ObservationDefinitionInputViewModel DefinitionVM { get; set; } = new ObservationDefinitionInputViewModel();

        public SelectList ObservationTypes { get; set; }
        public SelectList ScientificDisciplines { get; set; }
        public SelectList MetricTypes { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            DefinitionVM = await _definitionService.GetDefinitionViewModelByIdAsync(id);
            if (DefinitionVM == null)
            {
                return NotFound();
            }

            await EnsureSelectLists();

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
                await _definitionService.UpdateDefinitionAsync(DefinitionVM);
                TempData["SuccessMessage"] = "Observation definition updated successfully.";
                return RedirectToPage("./Index");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the observation definition. Please try again.";
                // Log the error (assuming ILogger is injected)
                // _logger.LogError(ex, "Error updating ObservationDefinition with ID {Id}", DefinitionVM.Id);
                await EnsureSelectLists();
                return Page();
            }
        }

        private async Task EnsureSelectLists()
        {
            ObservationTypes = await _selectListService.GetObservationTypesSelectListAsync();
            ScientificDisciplines = await _selectListService.GetScientificDisciplinesSelectListAsync();
            MetricTypes = await _selectListService.GetMetricTypesSelectListAsync(null, null, DefinitionVM.MetricTypeIds);
        }
    }
}