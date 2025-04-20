
namespace BiermanTech.CriticalDog.Analytics
{
    public interface IWeightAnalyticsProvider
    {
        Task<WeightChangeReport> GetWeightChangeReportAsync(int subjectId);
    }
}