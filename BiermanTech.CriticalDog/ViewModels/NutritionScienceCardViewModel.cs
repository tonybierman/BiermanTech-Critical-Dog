using BiermanTech.CanineHealth;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Interfaces;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class NutritionScienceCardViewModel : IDisciplineCardViewModel
    {
        public EnergyCalculationResult? Result { get; set; } = default!;
        public TrendReportViewModel WeightReport { get; set; } = new TrendReportViewModel();
        public AnalyticsReportPartialViewModel AnalyticPartialVM { get; set; } = new AnalyticsReportPartialViewModel();
        public SubjectRecord? WeightRecord { get; internal set; }
        public SubjectRecord? LifestageRecord { get; internal set; }
        public SubjectRecord? IdealWeightRecord { get; internal set; }
        public string Title => "Nutrition";
        public int SubjectId { get; set; }
    }
}
