using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Services
{
    public interface ISelectListService
    {
        Task<SelectList> GetObservationDefinitionsSelectListAsync();
        Task<SelectList> GetObservationDefinitionsSelectListAsync(int? selectedId = null);

        Task<SelectList> GetUnitsSelectListAsync();

        Task<SelectList> GetMetricTypesSelectListAsync(int observationDefinitionId, int? selectedId = null);
        Task<SelectList> GetMetricTypesSelectListAsync(IEnumerable<int>? selectedIds = null);

        Task<SelectList> GetObservationTypesSelectListAsync();
        Task<SelectList> GetScientificDisciplinesSelectListAsync();

        Task<SelectList> GetMetaTagsSelectListAsync();
        Task<SelectList> GetMetaTagsSelectListAsync(IEnumerable<int>? selectedIds = null);

        Task<SelectList> GetSubjectsSelectListAsync();
        Task<SelectList> GetSubjectTypesSelectListAsync();


    }
}