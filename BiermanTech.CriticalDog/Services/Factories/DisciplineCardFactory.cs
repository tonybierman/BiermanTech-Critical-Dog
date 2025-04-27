using BiermanTech.CriticalDog.Services.DisciplineCardProviders;
using BiermanTech.CriticalDog.Services.Interfaces;

namespace BiermanTech.CriticalDog.Services.Factories
{
    public class DisciplineCardFactory : IDisciplineCardFactory
    {
        private readonly ISubjectRecordService _recordService;
        private readonly IEnergyCalculationService _calcService;
        private readonly IObservationAnalyticsProvider _analyticsProvider;
        private readonly IUnitConverter _unitConverter;

        public DisciplineCardFactory(ISubjectRecordService recordService, IUnitConverter unitConverter, IEnergyCalculationService calcService, IObservationAnalyticsProvider analyticsProvider) 
        {
            _recordService = recordService;
            _calcService = calcService;
            _analyticsProvider = analyticsProvider;
            _unitConverter = unitConverter;
        }

        public IDisciplineCardProvider? CreateProvider(int subjectId, string slug)
        {
            if (slug == null)
                return null;

            return slug switch
            {
                "NutritionScience" => new NutritionScienceCardProvider(subjectId, _recordService, _unitConverter, _calcService, _analyticsProvider),
                _ => throw new ArgumentException($"Unsupported provider type: {slug}")
            };
        }
    }
}
