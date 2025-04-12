using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;

namespace BiermanTech.CriticalDog.Services
{
    public interface ISubjectTypeService
    {
        Task CreateSubjectTypeAsync(SubjectTypeInputViewModel viewModel);
        Task DeleteSubjectTypeAsync(int id);
        Task<List<SubjectTypeInputViewModel>> GetAllSubjectTypesAsync();
        Task<SubjectType> GetSubjectTypeByIdAsync(int id);
        Task<SubjectTypeInputViewModel> GetSubjectTypeViewModelByIdAsync(int id);
        Task UpdateSubjectTypeAsync(SubjectTypeInputViewModel viewModel);
    }
}