using BiermanTech.CriticalDog.Analytics;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class NutritionSciencePartialViewModel
    {
        public ObservationChangeReport WeightReport { get; set; } = new ObservationChangeReport();
        public AnalyticsReportPartialViewModel AnalyticPartialVM { get; set; } = new AnalyticsReportPartialViewModel();
    }
}
