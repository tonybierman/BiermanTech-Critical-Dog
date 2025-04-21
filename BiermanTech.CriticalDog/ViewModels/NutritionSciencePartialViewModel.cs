using BiermanTech.CriticalDog.Analytics;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class NutritionSciencePartialViewModel
    {
        public TrendReport WeightReport { get; set; } = new TrendReport();
        public AnalyticsReportPartialViewModel AnalyticPartialVM { get; set; } = new AnalyticsReportPartialViewModel();
    }
}
