using BiermanTech.CriticalDog.ViewModels;

namespace BiermanTech.CriticalDog.Services.Interfaces
{
    public interface IObservationAnalyticsProvider
    {
        Task<TrendReportViewModel> GetObservationChangeReportAsync(int subjectId, string observationDefinitionName, string? displayUnitName = null);
    }
}