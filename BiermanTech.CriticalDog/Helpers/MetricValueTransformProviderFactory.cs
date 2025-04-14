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
                case 6:
                    return new OfaHipGradeMetricValueTransformProvider();
                default:
                    return null;
            }
        }
    }
}
