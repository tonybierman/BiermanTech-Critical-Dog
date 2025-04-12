using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Pages.MetricTypes
{
    public class EditModel : PageModel
    {
        private readonly IMetricTypeService _metricTypeService;

        public EditModel(IMetricTypeService metricTypeService)
        {
            _metricTypeService = metricTypeService;
        }

        [BindProperty]
        public MetricTypeInputViewModel MetricTypeVM { get; set; } = new MetricTypeInputViewModel();

        public SelectList ObservationDefinitions { get; set; }
        public SelectList Units { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            MetricTypeVM = await _metricTypeService.GetMetricTypeViewModelByIdAsync(id);
            if (MetricTypeVM == null)
            {
                return NotFound();
            }

            ObservationDefinitions = await _metricTypeService.GetObservationDefinitionsSelectListAsync();
            Units = await _metricTypeService.GetUnitsSelectListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ObservationDefinitions = await _metricTypeService.GetObservationDefinitionsSelectListAsync();
                Units = await _metricTypeService.GetUnitsSelectListAsync();
                return Page();
            }

            try
            {
                await _metricTypeService.UpdateMetricTypeAsync(MetricTypeVM);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}