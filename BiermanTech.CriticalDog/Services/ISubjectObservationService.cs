using BiermanTech.CriticalDog.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Services
{
    public interface ISubjectObservationService
    {
        Task<Subject?> GetByIdAsync(int id);
        Task<MetricType?> GetMetricTypeByIdAsync(int? metricTypeId);
        Task<ObservationDefinition?> GetObservationDefinitionByIdAsync(int? observationDefinitionId);
        Task<int> SaveSubjectRecordAsync(SubjectRecord record, IEnumerable<int>? selectedMetaTagIds);
    }
}