namespace BiermanTech.CriticalDog.Helpers
{
    public static class MetricValueTransformProviderFactory
    {
        public static IMetricValueTransformProvider? GetProvider(int observationTypeId)
        {
            switch (observationTypeId)
            {
                case 5:
                    return new LifeStageFactorsMetricValueTransformProvider();
                // Add more cases for other ObservationTypeIds
                default:
                    return null;
            }
        }
    }
}
