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
        private readonly ISelectListService _selectListService;

        public CreateModel(IMetricTypeService metricTypeService, ISelectListService selectListService)
        {
            _metricTypeService = metricTypeService;
            _selectListService = selectListService;
        }

        [BindProperty]
        public MetricTypeInputViewModel MetricTypeVM { get; set; } = new MetricTypeInputViewModel();

        public SelectList ObservationDefinitions { get; set; }
        public SelectList Units { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ObservationDefinitions = await _selectListService.GetObservationDefinitionsSelectListAsync();
            Units = await _selectListService.GetUnitsSelectListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ObservationDefinitions = await _selectListService.GetObservationDefinitionsSelectListAsync();
                Units = await _selectListService.GetUnitsSelectListAsync();
                return Page();
            }

            await _metricTypeService.CreateMetricTypeAsync(MetricTypeVM);
            return RedirectToPage("./Index");
        }
    }
}