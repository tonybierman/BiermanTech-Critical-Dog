using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Services
{
    public interface ISubjectService
    {
        Task CreateSubjectAsync(SubjectInputViewModel viewModel);
        Task DeleteSubjectAsync(int id);
        Task<Subject> GetSubjectByIdAsync(int id);
        Task<SelectList> GetSubjectTypesSelectListAsync();
        Task<SubjectInputViewModel> GetSubjectViewModelByIdAsync(int id);
        Task UpdateSubjectAsync(SubjectInputViewModel viewModel);
    }
}