using BiermanTech.CanineHealth;
using BiermanTech.CriticalDog.Data;

namespace BiermanTech.CriticalDog.Reports.Columns
{
    public static class MetricValueTransformerFactory
    {
        // Dictionary to map observation type names to provider instances
        private static readonly Dictionary<string, IMetricValueTransformProvider> _providers = new()
        {
            { "LifeStageFactor", new MetricValueTransformer<LifeStageFactorsEnum>() },
            { "EstrusStage", new MetricValueTransformer<EstrusStageEnum>() },
            { "OfaHipsGrade", new MetricValueTransformer<OfaHipsGradeEnum>() },
            { "GeneticHealthConditionStatus", new MetricValueTransformer<GeneticHealthConditionStatusEnum>() },
            { "BodyConditionScore", new MetricValueTransformer<BodyConditionScoreEnum>() },
            { "StoolQualityScore", new MetricValueTransformer<StoolQualityScoreEnum>() },
            { "ExerciseIntensity", new MetricValueTransformer<ExerciseIntensityLevelEnum>() },
            { "Sex", new MetricValueTransformer<SexEnum>() },
            { "BehaviorCategory", new MetricValueTransformer<CanineBehaviorCategoryEnum>() },
            { "CoatCondition", new MetricValueTransformer<CoatConditionEnum>() }
        };

        /// <summary>
        /// Gets the appropriate IMetricValueTransformProvider based on the observation definition.
        /// </summary>
        /// <param name="observationDefinition">The observation definition containing the observation type.</param>
        /// <returns>An IMetricValueTransformProvider or null if no matching provider is found.</returns>
        public static IMetricValueTransformProvider? GetProvider(ObservationDefinition? observationDefinition)
        {
            if (observationDefinition?.ObservationType?.Name is not string typeName)
            {
                return null;
            }

            return _providers.TryGetValue(typeName, out var provider) ? provider : null;
        }
    }
}