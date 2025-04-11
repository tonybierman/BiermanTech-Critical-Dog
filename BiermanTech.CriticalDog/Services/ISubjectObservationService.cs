using BiermanTech.CriticalDog.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Services
{
    public interface ISubjectObservationService
    {
        Task<Subject?> GetByIdAsync(int id);
        Task<SelectList> GetMetaTagsSelectListAsync(IEnumerable<int>? selectedIds = null);
        Task<MetricType?> GetMetricTypeByIdAsync(int? metricTypeId);
        Task<SelectList> GetMetricTypesSelectListAsync(int observationDefinitionId, int? selectedId = null);
        Task<ObservationDefinition?> GetObservationDefinitionByIdAsync(int? observationDefinitionId);
        Task<SelectList> GetObservationDefinitionsSelectListAsync(int? selectedId = null);
        Task SaveSubjectRecordAsync(SubjectRecord record, IEnumerable<int>? selectedMetaTagIds);
    }
}