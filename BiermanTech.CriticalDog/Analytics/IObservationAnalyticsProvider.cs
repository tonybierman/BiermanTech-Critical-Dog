namespace BiermanTech.CriticalDog.Analytics
{
    public interface IObservationAnalyticsProvider
    {
        Task<TrendReport> GetObservationChangeReportAsync(int subjectId, string observationDefinitionName, string? displayUnitName = null);
    }
}