using System.Linq;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.Step2
{
    public class ProcessMetricTypeHandler : ICreateStep2PostHandler
    {
        public async Task HandleAsync(CreateStep2PostContext context)
        {
            if (!context.ObservationVM.MetricTypeId.HasValue && context.ObservationDefinition.MetricTypes.Any())
            {
                if (context.ObservationDefinition.MetricTypes.Count() == 1)
                {
                    context.ObservationVM.MetricTypeId = context.ObservationDefinition.MetricTypes.First().Id;
                }
                else
                {
                    context.PageModel.ModelState.AddModelError("ObservationVM.MetricTypeId", "Please select a metric type for quantitative observations.");
                    context.Result = context.PageModel.Page();
                }
            }
        }
    }
}