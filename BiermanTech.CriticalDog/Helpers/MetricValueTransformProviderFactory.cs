namespace BiermanTech.CriticalDog.Helpers
{
    public static class MetricValueTransformProviderFactory
    {
        public static IMetricValueTransformProvider? GetProvider(int observationTypeId)
        {
            switch (observationTypeId)
            {
                case 5:
                    return new CanineLifeStageFactorsMetricValueTransformProvider();
                case 6:
                    return new CanineOfaHipGradeMetricValueTransformProvider();
                default:
                    return null;
            }
        }
    }
}
