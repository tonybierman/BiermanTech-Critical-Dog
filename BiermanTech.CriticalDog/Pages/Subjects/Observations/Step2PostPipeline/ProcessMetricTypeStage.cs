using System.Linq;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.Step2PostPipeline
{
    public class ProcessMetricTypeStage : ICreateStep2PostStage
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