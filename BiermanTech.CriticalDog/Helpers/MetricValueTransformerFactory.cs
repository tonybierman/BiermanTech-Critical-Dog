using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Helpers.BiermanTech.CriticalDog.Helpers;
using System;
using System.Collections.Generic;

namespace BiermanTech.CriticalDog.Helpers
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
            { "BodyConditionScore", new MetricValueTransformer<BodyConditionScoreEnum>() }
        };

        /// <summary>
        /// Gets the appropriate IMetricValueTransformProvider based on the observation definition.
        /// </summary>
        /// <param name="observationDefinition">The observation definition containing the observation type.</param>
        /// <returns>An IMetricValueTransformProvider or null if no matching provider is found.</returns>
        public static IMetricValueTransformProvider? GetProvider(ObservationDefinition? observationDefinition)
        {
            if (observationDefinition?.ObservationType?.TypeName is not string typeName)
            {
                return null;
            }

            return _providers.TryGetValue(typeName, out var provider) ? provider : null;
        }
    }
}