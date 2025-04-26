using BiermanTech.CanineHealth;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Helpers;
using BiermanTech.CriticalDog.Services.Interfaces;

namespace BiermanTech.CriticalDog.Services.Factories
{
    public class MetricValueTransformerFactory : IMetricValueTransformerFactory
    {
        /// <summary>
        /// Gets the appropriate IMetricValueTransformProvider based on the observation definition.
        /// </summary>
        /// <param name="observationDefinition">The observation definition containing the observation type.</param>
        /// <returns>An IMetricValueTransformProvider or null if no matching provider is found.</returns>
        public IMetricValueTransformProvider? GetProvider(ObservationDefinition? observationDefinition)
        {
            return observationDefinition?.ObservationType?.Name switch
            {
                "LifeStageFactor" => new MetricValueTransformer<LifeStageFactorsEnum>(),
                "EstrusStage" => new MetricValueTransformer<EstrusStageEnum>(),
                "OfaHipsGrade" => new MetricValueTransformer<OfaHipsGradeEnum>(),
                "GeneticHealthConditionStatus" => new MetricValueTransformer<GeneticHealthConditionStatusEnum>(),
                "BodyConditionScore" => new MetricValueTransformer<BodyConditionScoreEnum>(),
                "StoolQualityScore" => new MetricValueTransformer<StoolQualityScoreEnum>(),
                "ExerciseIntensity" => new MetricValueTransformer<ExerciseIntensityLevelEnum>(),
                "Sex" => new MetricValueTransformer<SexEnum>(),
                "BehaviorCategory" => new MetricValueTransformer<CanineBehaviorCategoryEnum>(),
                "CoatCondition" => new MetricValueTransformer<CoatConditionEnum>(),
                _ => null
            };
        }
    }
}