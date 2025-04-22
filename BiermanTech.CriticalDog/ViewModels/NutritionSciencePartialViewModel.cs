using BiermanTech.CriticalDog.Analytics;
using BiermanTech.CriticalDog.Data;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class NutritionSciencePartialViewModel
    {
        public TrendReport WeightReport { get; set; } = new TrendReport();
        public AnalyticsReportPartialViewModel AnalyticPartialVM { get; set; } = new AnalyticsReportPartialViewModel();
        public SubjectRecord IdealWeightRecord { get; internal set; }
        public SubjectRecord WeightRecord { get; internal set; }
        public SubjectRecord LifestageRecord { get; internal set; }
    }
}
