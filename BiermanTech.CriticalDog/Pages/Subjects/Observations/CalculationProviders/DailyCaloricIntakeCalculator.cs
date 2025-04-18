using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.CalculationProviders
{
    public class DailyCaloricIntakeCalculator : IMetricValueCalculatorProvider
    {
        public bool CanHandle(Subject dog, CreateObservationViewModel observationVM)
        {
            return true;
        }

        public void Execute(Subject dog, CreateObservationViewModel observationVM)
        {

            var item = observationVM.MetricTypes.FirstOrDefault();
            if (item != null && int.TryParse(item.Value, out int id))
            {
                observationVM.MetricTypeId = id;
            }

            observationVM.MetricValue = 69;
        }
    }
}
