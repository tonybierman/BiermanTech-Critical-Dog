using AutoMapper;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace BiermanTech.CriticalDog.Pages.Admin.MetricTypes
{
    public class DetailsModel : PageModel
    {
        private readonly IMetricTypeService _metricTypeService;
        private readonly IMapper _mapper;

        public DetailsModel(IMetricTypeService metricTypeService, IMapper mapper)
        {
            _metricTypeService = metricTypeService;
            _mapper = mapper;
        }

        public MetricTypeInputViewModel MetricTypeVM { get; set; } = new MetricTypeInputViewModel();
        public string ObservationDefinitionNames { get; set; }
        public string UnitName { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var metricType = await _metricTypeService.GetMetricTypeByIdAsync(id);
            if (metricType == null)
            {
                return NotFound();
            }

            MetricTypeVM = _mapper.Map<MetricTypeInputViewModel>(metricType);
            ObservationDefinitionNames = metricType.ObservationDefinitions.Any()
                ? string.Join(", ", metricType.ObservationDefinitions.Select(od => od.Name))
                : "None";
            UnitName = metricType.Unit?.Name ?? "Unknown";

            return Page();
        }
    }
}