using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.CalculationProviders
{
    public interface IMetricValueCalculatorProvider
    {
        bool CanHandle(Subject dog, CreateObservationViewModel observationVM);
        void Execute(Subject dog, CreateObservationViewModel observationVM);
    }
}
