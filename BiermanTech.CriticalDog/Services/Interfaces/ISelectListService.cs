using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Services.Interfaces
{
    public interface ISelectListService
    {
        Task<SelectList> GetMetaTagsSelectListAsync(IEnumerable<int>? selectedIds = null, bool filterActive = true);
        Task<SelectList> GetMetricTypesSelectListAsync(int? observationDefinitionId = null, int? selectedId = null, IEnumerable<int>? selectedIds = null, bool filterActive = true);
        Task<SelectList> GetObservationDefinitionsSelectListAsync(int? selectedId = null, bool filterActive = true);
        Task<SelectList> GetObservationTypesSelectListAsync(bool filterActive = true);
        Task<SelectList> GetScientificDisciplinesSelectListAsync(bool filterActive = true);
        Task<SelectList> GetSubjectsSelectListAsync();
        Task<SelectList> GetSubjectTypesSelectListAsync();
        Task<SelectList> GetUnitsSelectListAsync(bool filterActive = true);
    }
}