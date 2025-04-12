using AutoMapper;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.MetricTypes
{
    public class DeleteModel : PageModel
    {
        private readonly IMetricTypeService _metricTypeService;
        private readonly IMapper _mapper;

        public DeleteModel(IMetricTypeService metricTypeService, IMapper mapper)
        {
            _metricTypeService = metricTypeService;
            _mapper = mapper;
        }

        [BindProperty]
        public MetricTypeInputViewModel MetricTypeVM { get; set; } = new MetricTypeInputViewModel();
        public string ObservationDefinitionName { get; set; }
        public string UnitName { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var metricType = await _metricTypeService.GetMetricTypeByIdAsync(id);
            if (metricType == null)
            {
                return NotFound();
            }

            MetricTypeVM = _mapper.Map<MetricTypeInputViewModel>(metricType);
            ObservationDefinitionName = metricType.ObservationDefinition?.DefinitionName ?? "Unknown";
            UnitName = metricType.Unit?.UnitName ?? "Unknown";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _metricTypeService.DeleteMetricTypeAsync(MetricTypeVM.Id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}