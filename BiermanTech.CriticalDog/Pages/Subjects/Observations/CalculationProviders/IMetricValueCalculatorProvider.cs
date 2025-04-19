using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.CalculationProviders
{
    public interface IMetricValueCalculatorProvider
    {
        string Slug { get; }
        Task<bool> CanHandle(Subject dog, CreateObservationViewModel observationVM);
        Task Execute(Subject dog, CreateObservationViewModel observationVM);
    }
}
