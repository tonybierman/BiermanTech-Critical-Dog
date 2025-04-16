namespace BiermanTech.CriticalDog.Helpers
{
    public static class MetricValueTransformProviderFactory
    {
        public static IMetricValueTransformProvider? GetProvider(string observationTypeName)
        {
            switch (observationTypeName)
            {
                case "CanineLifeStageFactor":
                    return new CanineLifeStageFactorsMetricValueTransformProvider();
                case "CanineOfaHipGrade":
                    return new CanineOfaHipGradeMetricValueTransformProvider();
                case "CanineGeneticHealthConditionStatus":
                    return new CanineGeneticHealthConditionStatusMetricValueTransformProvider();
                default:
                    return null;
            }
        }
    }
}
