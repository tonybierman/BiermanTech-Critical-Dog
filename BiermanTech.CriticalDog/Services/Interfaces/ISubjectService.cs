using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<int> CreateSubjectAsync(SubjectInputViewModel viewModel);
        Task<int> DeleteSubjectAsync(int id);
        Task<IList<Subject>> GetFilteredSubjectsAsync();
        Task<List<SubjectViewModel>> GetFilteredSubjectViewModelsAsync();
        Task<Subject> GetSubjectByIdAsync(int id);
        Task<SubjectInputViewModel> GetSubjectViewModelByIdAsync(int id);
        Task<int> UpdateSubjectAsync(SubjectInputViewModel viewModel);
    }
}