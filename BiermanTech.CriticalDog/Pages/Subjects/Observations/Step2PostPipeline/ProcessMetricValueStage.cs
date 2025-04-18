using BiermanTech.CriticalDog.Services;
using System.Linq;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.Step2PostPipeline
{
    public class ProcessMetricValueStage : ICreateStep2PostStage
    {
        private readonly ISubjectObservationService _service;

        public ProcessMetricValueStage(ISubjectObservationService service)
        {
            _service = service;
        }

        public async Task HandleAsync(CreateStep2PostContext context)
        {
            if (!context.ObservationVM.MetricValue.HasValue)
            {
                if (context.SelectedListItems?.Any() == true && context.SelectedItem > 0)
                {
                    context.ObservationVM.MetricValue = context.SelectedItem;
                }

                if (!context.ObservationVM.MetricValue.HasValue)
                {
                    context.ObservationVM.MetricValue = 0m;
                }
            }

            if (context.ObservationVM.MetricValue.HasValue &&
                (context.ObservationVM.MetricValue < context.ObservationDefinition.MinimumValue ||
                 context.ObservationVM.MetricValue > context.ObservationDefinition.MaximumValue))
            {
                context.PageModel.ModelState.AddModelError("ObservationVM.MetricValue",
                    $"Value must be between {context.ObservationDefinition.MinimumValue} and {context.ObservationDefinition.MaximumValue}.");
                context.ObservationVM.MetricTypes = await _service.GetMetricTypesSelectListAsync(context.ObservationVM.ObservationDefinitionId.Value);
                context.Result = context.PageModel.Page();
            }
        }
    }
}