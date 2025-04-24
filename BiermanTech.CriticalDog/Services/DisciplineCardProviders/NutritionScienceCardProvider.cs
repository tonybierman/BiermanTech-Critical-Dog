using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.EntityServices;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;

namespace BiermanTech.CriticalDog.Services.DisciplineCardProviders
{
    public class NutritionScienceCardProvider : IDisciplineCardProvider
    {
        private readonly ISubjectRecordService _recordService;
        private readonly IEnergyCalculationService _calcService;
        private readonly IObservationAnalyticsProvider _analyticsProvider;
        private readonly int _subjectId;

        public string PartialName => "_NutritionScienceCardPartial";
        public IDisciplineCardViewModel ViewModel { get ; set ; } = default!;

        public NutritionScienceCardProvider(int subjectId, ISubjectRecordService recordService, IEnergyCalculationService calcService, IObservationAnalyticsProvider analyticsProvider)
        {
            _recordService = recordService;
            _calcService = calcService;
            _analyticsProvider = analyticsProvider;
            _subjectId = subjectId;
        }

        public bool CanHandle()
        {
            if (ViewModel is NutritionScienceCardViewModel vm)
            {
                return (vm.Result != null && vm.Result.IsValid) || (vm.WeightReport != null && vm.WeightReport.Records.Any() == true);
            }

            return false;
        }

        public async Task Init()
        {
            var records = await _recordService.GetMostRecentSubjectRecordsByDisciplineAsync(_subjectId, "WeighIn");
            var weightReport = await _analyticsProvider.GetObservationChangeReportAsync(_subjectId, "WeighIn");

            var vm = new NutritionScienceCardViewModel()
            {
                IdealWeightRecord = records?.Where(r => r.ObservationDefinition.Name == "IdealWeight")?.FirstOrDefault(),
                WeightRecord = records?.Where(r => r.ObservationDefinition.Name == "WeighIn")?.FirstOrDefault(),
                LifestageRecord = records?.Where(r => r.ObservationDefinition.Name == "CanineLifeStageFactor")?.FirstOrDefault(),
                WeightReport = weightReport,
                AnalyticPartialVM = new AnalyticsReportPartialViewModel() { Report = weightReport }
            };

            var input = new EnergyCalculationInput
            {
                WeightMetricValue = vm.IdealWeightRecord?.MetricValue ?? vm.WeightRecord?.MetricValue,
                UnitName = vm.WeightRecord?.MetricType?.Unit?.Name,
                LifeStageMetricValue = vm.LifestageRecord?.MetricValue
            };

            // Call the service
            vm.Result = await _calcService.CalculateEnergyRequirementsAsync(input);


            ViewModel = vm;

        }
    }
}
