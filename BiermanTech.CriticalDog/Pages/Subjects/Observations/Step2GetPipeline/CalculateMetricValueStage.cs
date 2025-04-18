using BiermanTech.CriticalDog.Pages.Subjects.Observations.CalculationProviders;
using BiermanTech.CriticalDog.Services;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations.Step2GetPipeline
{
    public class CalculateMetricValueStage : ICreateStep2GetStage
    {
        private readonly ISubjectObservationService _service;

        public CalculateMetricValueStage(ISubjectObservationService service)
        {
            _service = service;
        }

        public async Task HandleAsync(Step2GetContext context)
        {
            if (!context.ObservationVM.MetricValue.HasValue)
            {
                var calculator = MetricValueCalculatorFactory.GetProvider(context.ObservationDefinitionName);
                if (calculator != null && calculator.CanHandle(context.Dog, context.ObservationVM))
                {
                    calculator.Execute(context.Dog, context.ObservationVM);
                }
            }
        }
    }

}