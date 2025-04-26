using BiermanTech.CriticalDog.Data;

namespace BiermanTech.CriticalDog.Services.Interfaces
{
    public interface IMetricValueTransformerFactory
    {
        IMetricValueTransformProvider? GetProvider(ObservationDefinition? observationDefinition);
    }
}