using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Services
{
    public interface ISubjectService
    {
        Task<int> CreateSubjectAsync(SubjectInputViewModel viewModel);
        Task<int> DeleteSubjectAsync(int id);
        Task<Subject> GetSubjectByIdAsync(int id);
        Task<SelectList> GetSubjectTypesSelectListAsync();
        Task<SubjectInputViewModel> GetSubjectViewModelByIdAsync(int id);
        Task<int> UpdateSubjectAsync(SubjectInputViewModel viewModel);
    }
}