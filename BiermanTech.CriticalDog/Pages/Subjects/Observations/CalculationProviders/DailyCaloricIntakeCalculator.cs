using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Helpers;
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

            // TODO: Pull parms from dog's history or don't set
            observationVM.MetricValue = (decimal)CanineMerCalculator.CalculateMeanMer(LifeStageFactorsEnum.IntactAdult, 45);
        }
    }
}
