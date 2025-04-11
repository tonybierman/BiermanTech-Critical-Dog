using BiermanTech.CriticalDog.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Services
{
    public interface IDogObservationService
    {
        Task<Dog?> GetDogByIdAsync(int dogId);
        Task<SelectList> GetMetaTagsSelectListAsync(IEnumerable<int>? selectedIds = null);
        Task<MetricType?> GetMetricTypeByIdAsync(int? metricTypeId);
        Task<SelectList> GetMetricTypesSelectListAsync(int observationDefinitionId, int? selectedId = null);
        Task<ObservationDefinition?> GetObservationDefinitionByIdAsync(int? observationDefinitionId);
        Task<SelectList> GetObservationDefinitionsSelectListAsync(int? selectedId = null);
        Task SaveDogRecordAsync(DogRecord dogRecord, IEnumerable<int>? selectedMetaTagIds);
    }
}