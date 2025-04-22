using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Services
{
    public interface ISubjectRecordService
    {
        Task CreateSubjectRecordAsync(SubjectRecordInputViewModel viewModel);
        Task DeleteSubjectRecordAsync(int id);
        Task<List<SubjectRecordInputViewModel>> GetAllSubjectRecordsAsync();
        Task<SubjectRecord> GetMostRecentSubjectRecordAsync(int subjectId, string definitionName);
        Task<SubjectRecord> GetSubjectRecordByIdAsync(int id);
        Task<SubjectRecordInputViewModel> GetSubjectRecordViewModelByIdAsync(int id);
        Task UpdateSubjectRecordAsync(SubjectRecordInputViewModel viewModel);
    }
}