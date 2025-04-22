using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.SubjectRecords
{
    public class IndexModel : PageModel
    {
        private readonly ISubjectRecordService _recordService;

        public IndexModel(ISubjectRecordService recordService)
        {
            _recordService = recordService;
        }

        public List<SubjectRecordInputViewModel> Records { get; set; } = new List<SubjectRecordInputViewModel>();
        public Dictionary<int, string> SubjectNames { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, string> ObservationDefinitionNames { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, List<string>> MetaTagNames { get; set; } = new Dictionary<int, List<string>>();

        public async Task OnGetAsync()
        {
            Records = await _recordService.GetAllSubjectRecordsAsync();

            foreach (var record in Records)
            {
                var entity = await _recordService.GetSubjectRecordByIdAsync(record.Id);
                SubjectNames[record.Id] = entity.Subject?.Name ?? "Unknown";
                ObservationDefinitionNames[record.Id] = entity.ObservationDefinition?.Name ?? "Unknown";
                MetaTagNames[record.Id] = entity.MetaTags.Select(m => m.Name).ToList();
            }
        }
    }
}