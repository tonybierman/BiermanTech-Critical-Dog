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
        Task<SelectList> GetMetaTagsSelectListAsync();
        Task<SelectList> GetMetricTypesSelectListAsync();
        Task<SubjectRecord> GetMostRecentSubjectRecordAsync(int subjectId, string definitionName);
        Task<SelectList> GetObservationDefinitionsSelectListAsync();
        Task<SubjectRecord> GetSubjectRecordByIdAsync(int id);
        Task<SubjectRecordInputViewModel> GetSubjectRecordViewModelByIdAsync(int id);
        Task<SelectList> GetSubjectsSelectListAsync();
        Task UpdateSubjectRecordAsync(SubjectRecordInputViewModel viewModel);
    }
}