namespace BiermanTech.CriticalDog.Analytics
{
    public interface IObservationAnalyticsProvider
    {
        Task<ObservationChangeReport> GetObservationChangeReportAsync(int subjectId, string observationDefinitionName, string? displayUnitName = null);
    }
}