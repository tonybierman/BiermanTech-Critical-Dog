using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Helpers;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.CalculationProviders
{
    public class DailyCaloricIntakeCalculator : IMetricValueCalculatorProvider
    {
        private ISubjectRecordService _service;

        public string Slug => "DailyCaloricIntake";

        public DailyCaloricIntakeCalculator(ISubjectRecordService service)
        {
            _service = service;
        }

        public async Task<bool> CanHandle(Subject dog, CreateObservationViewModel observationVM)
        {
            var weightRecord = await _service.GetMostRecentSubjectRecordAsync(dog.Id, "WeighIn");
            var lifeStageRecord = await _service.GetMostRecentSubjectRecordAsync(dog.Id, "CanineLifeStageFactor");

            return weightRecord != null && lifeStageRecord != null;
        }

        public async Task Execute(Subject dog, CreateObservationViewModel observationVM)
        {
            var weightRecord = await _service.GetMostRecentSubjectRecordAsync(dog.Id, "WeighIn");
            var lifeStageRecord = await _service.GetMostRecentSubjectRecordAsync(dog.Id, "CanineLifeStageFactor");

            var item = observationVM.MetricTypes.FirstOrDefault();
            if (item != null && int.TryParse(item.Value, out int id))
            {
                observationVM.MetricTypeId = id;
            }

            observationVM.MetricValue = (decimal)CanineMerCalculator.CalculateMeanMer((LifeStageFactorsEnum)lifeStageRecord.MetricValue, (double)weightRecord.MetricValue);
        }
    }
}
