using BiermanTech.CriticalDog.Data;

namespace BiermanTech.CriticalDog.Helpers
{
    public static class MetricValueTransformProviderFactory
    {
        public static IMetricValueTransformProvider? GetProvider(ObservationDefinition? observationDefinition)
        {
            if (observationDefinition == null)
            {
                return null;
            }

            switch (observationDefinition.DefinitionName)
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
