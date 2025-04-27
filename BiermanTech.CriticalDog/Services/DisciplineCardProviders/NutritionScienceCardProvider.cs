using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Newtonsoft.Json.Linq;

namespace BiermanTech.CriticalDog.Services.DisciplineCardProviders
{
    public class NutritionScienceCardProvider : IDisciplineCardProvider
    {
        private readonly IUnitConverter _unitConverter;
        private readonly ISubjectRecordService _recordService;
        private readonly IEnergyCalculationService _calcService;
        private readonly IObservationAnalyticsProvider _analyticsProvider;
        private readonly int _subjectId;

        public string PartialName => "_NutritionScienceCardPartial";
        public IDisciplineCardViewModel ViewModel { get ; set ; } = default!;

        public NutritionScienceCardProvider(int subjectId, ISubjectRecordService recordService, IUnitConverter unitConverter, IEnergyCalculationService calcService, IObservationAnalyticsProvider analyticsProvider)
        {
            _unitConverter = unitConverter;
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
            // Fetch records
            var records = (await _recordService.GetMostRecentSubjectRecordsByDisciplineAsync(_subjectId, "NutritionScience"))
                ?.ToList() ?? [];

            // Extract specific records
            var recordsByObservation = records
                .GroupBy(r => r.ObservationDefinition.Name)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault());

            var idealWeightRecord = recordsByObservation.GetValueOrDefault("IdealWeight");
            var weightRecord = recordsByObservation.GetValueOrDefault("WeighIn");
            var lifestageRecord = recordsByObservation.GetValueOrDefault("LifeStage");

            // Fetch weight report
            var weightReport = await _analyticsProvider.GetObservationChangeReportAsync(_subjectId, "WeighIn");

            // Build view model
            var viewModel = new NutritionScienceCardViewModel
            {
                IdealWeightRecord = idealWeightRecord,
                WeightRecord = weightRecord,
                LifestageRecord = lifestageRecord,
                WeightReport = weightReport,
                AnalyticPartialVM = new AnalyticsReportPartialViewModel { Report = weightReport }
            };

            // Calculate energy requirements
            var input = new EnergyCalculationInput
            {
                WeightMetricValue = idealWeightRecord?.MetricValue ?? weightRecord?.MetricValue,
                UnitName = weightRecord?.MetricType?.Unit?.Name,
                LifeStageMetricValue = lifestageRecord?.MetricValue
            };

            viewModel.Result = await _calcService.CalculateEnergyRequirementsAsync(input);
            ViewModel = viewModel;

            // Calculate projected ideal weight date
            if (idealWeightRecord?.MetricValue.HasValue == true &&
                idealWeightRecord.MetricType?.Unit?.Name is string sourceUnitName &&
                weightReport?.AverageAmountPerDay.HasValue == true &&
                records.Any())
            {
                var idealValue = (decimal)idealWeightRecord.MetricValue.Value; // Convert double to decimal
                var dailyAmount = (decimal)weightReport.AverageAmountPerDay.Value; // Convert double to decimal
                var lastWeightRecord = records
                    .Where(r => r.ObservationDefinition.Name == "WeighIn")
                    .MaxBy(r => r.RecordTime);

                if (lastWeightRecord?.MetricValue.HasValue == true)
                {
                    var lastWeightValue = (decimal)lastWeightRecord.MetricValue.Value; // Convert double to decimal
                    var idealWeightValue = (double)idealWeightRecord.MetricValue.Value;
                    var convertedIdeal = await _unitConverter.ConvertAsync(sourceUnitName, weightReport.DisplayUnitName, idealWeightValue);
                    viewModel.IdealProjectedWhen = _calcService.ProjectIdealWeightDate(
                        (decimal)convertedIdeal,
                        lastWeightValue,
                        lastWeightRecord.RecordTime,
                        dailyAmount);
                }
            }
        }
    }
}
