using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Services
{
    public interface ISelectListService
    {
        Task<SelectList> GetMetricTypesSelectListAsync(IEnumerable<int>? selectedIds = null);
        Task<SelectList> GetObservationTypesSelectListAsync();
        Task<SelectList> GetScientificDisciplinesSelectListAsync();
    }
}