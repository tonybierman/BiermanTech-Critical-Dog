namespace BiermanTech.CriticalDog.Helpers
{
    public static class MetricValueProviderFactory
    {
        public static IMetricValueProvider GetProvider(int observationTypeId)
        {
            switch (observationTypeId)
            {
                case 5:
                    return new LifeStageFactorsMetricValueProvider();
                // Add more cases for other ObservationTypeIds
                default:
                    throw new NotSupportedException($"No provider found for ObservationTypeId: {observationTypeId}");
            }
        }
    }
}
