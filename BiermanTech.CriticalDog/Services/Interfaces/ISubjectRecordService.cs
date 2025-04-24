using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Services.Interfaces
{
    public interface ISubjectRecordService
    {
        Task CreateSubjectRecordAsync(SubjectRecordInputViewModel viewModel);
        Task DeleteSubjectRecordAsync(int id);
        Task<List<SubjectRecordInputViewModel>> GetAllSubjectRecordsAsync();
        Task<SubjectRecord> GetMostRecentSubjectRecordAsync(int subjectId, string definitionName);
        Task<IEnumerable<SubjectRecord>> GetMostRecentSubjectRecordsAsync(int subjectId);
        Task<IEnumerable<SubjectRecord>> GetMostRecentSubjectRecordsByDisciplineAsync(int subjectId, string? scientificDisciplineNameFilter);
        Task<SubjectRecord> GetSubjectRecordByIdAsync(int id);
        Task<SubjectRecordInputViewModel> GetSubjectRecordViewModelByIdAsync(int id);
        Task UpdateSubjectRecordAsync(SubjectRecordInputViewModel viewModel);
    }
}