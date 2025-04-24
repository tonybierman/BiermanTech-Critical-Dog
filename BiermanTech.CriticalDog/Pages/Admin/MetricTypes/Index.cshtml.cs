using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace BiermanTech.CriticalDog.Pages.Admin.MetricTypes
{
    public class IndexModel : PageModel
    {
        private readonly IMetricTypeService _metricTypeService;

        public IndexModel(IMetricTypeService metricTypeService)
        {
            _metricTypeService = metricTypeService;
        }

        public List<MetricTypeInputViewModel> MetricTypes { get; set; } = new List<MetricTypeInputViewModel>();
        public Dictionary<int, string> ObservationDefinitionNames { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, string> UnitNames { get; set; } = new Dictionary<int, string>();

        public async Task OnGetAsync()
        {
            MetricTypes = await _metricTypeService.GetAllMetricTypesAsync();

            foreach (var metricType in MetricTypes)
            {
                var entity = await _metricTypeService.GetMetricTypeByIdAsync(metricType.Id);
                ObservationDefinitionNames[metricType.Id] = entity.ObservationDefinitions.Any()
                    ? string.Join(", ", entity.ObservationDefinitions.Select(od => od.Name))
                    : "None";
                UnitNames[metricType.Id] = entity.Unit?.Name ?? "Unknown";
            }
        }
    }
}