using BiermanTech.CanineHealth;
using BiermanTech.CriticalDog.Analytics;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class NutritionScienceCardViewModel : ICardViewModel
    {

        private EnergyCalculationInput? _input = default!;
        private readonly IEnergyCalculationService _calcService;

        public EnergyCalculationResult? Result { get; private set; } = default!;
        public TrendReport WeightReport { get; set; } = new TrendReport();
        public AnalyticsReportPartialViewModel AnalyticPartialVM { get; set; } = new AnalyticsReportPartialViewModel();
        public SubjectRecord WeightRecord { get; internal set; }
        public SubjectRecord LifestageRecord { get; internal set; }
        public SubjectRecord IdealWeightRecord { get; internal set; }

        public NutritionScienceCardViewModel(IEnergyCalculationService calcService)
        {
            _calcService = calcService;
        }

        public string Title => "Nutrition";

        public bool CanHandle()
        {
            return (Result != null && Result.IsValid) || (WeightReport != null && WeightReport.Records.Any() == true);
        }

        public async Task Init()
        {
            // Create input record from view model
            var input = new EnergyCalculationInput
            {
                WeightMetricValue = IdealWeightRecord?.MetricValue ?? WeightRecord?.MetricValue,
                UnitName = WeightRecord?.MetricType?.Unit?.Name,
                LifeStageMetricValue = LifestageRecord?.MetricValue
            };

            // Call the service
            Result = await _calcService.CalculateEnergyRequirementsAsync(input);
        }
    }
}
