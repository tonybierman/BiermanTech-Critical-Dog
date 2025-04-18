using BiermanTech.CriticalDog.Data;

namespace BiermanTech.CriticalDog.Helpers
{
    public static class MetricValueTransformProviderFactory
    {
        public static IMetricValueTransformProvider? GetProvider(ObservationDefinition? observationDefinition)
        {
            if (observationDefinition?.ObservationType?.TypeName == null)
            {
                return null;
            }

            // This must key on the observation type
            switch (observationDefinition.ObservationType.TypeName)
            {
                case "LifeStageFactor":
                    return new LifeStageFactorsMetricValueTransformProvider();
                case "OfaHipsGrade":
                    return new OfaHipsGradeMetricValueTransformProvider();
                case "GeneticHealthConditionStatus":
                    return new GeneticHealthConditionStatusMetricValueTransformProvider();
                default:
                    return null;
            }
        }
    }
}
