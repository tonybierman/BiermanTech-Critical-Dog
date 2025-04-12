using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Pages.Admin.MetricTypes
{
    public class CreateModel : PageModel
    {
        private readonly IMetricTypeService _metricTypeService;

        public CreateModel(IMetricTypeService metricTypeService)
        {
            _metricTypeService = metricTypeService;
        }

        [BindProperty]
        public MetricTypeInputViewModel MetricTypeVM { get; set; } = new MetricTypeInputViewModel();

        public SelectList ObservationDefinitions { get; set; }
        public SelectList Units { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
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

            await _metricTypeService.CreateMetricTypeAsync(MetricTypeVM);
            return RedirectToPage("./Index");
        }
    }
}