using AutoMapper;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.SubjectRecords
{
    public class DetailsModel : PageModel
    {
        private readonly ISubjectRecordService _recordService;
        private readonly IMapper _mapper;

        public DetailsModel(ISubjectRecordService recordService, IMapper mapper)
        {
            _recordService = recordService;
            _mapper = mapper;
        }

        public SubjectRecordInputViewModel RecordVM { get; set; } = new SubjectRecordInputViewModel();
        public string SubjectName { get; set; }
        public string ObservationDefinitionName { get; set; }
        public int? MetricTypeId { get; set; }
        public List<string> MetaTagNames { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var record = await _recordService.GetSubjectRecordByIdAsync(id);
            if (record == null)
            {
                return NotFound();
            }

            RecordVM = _mapper.Map<SubjectRecordInputViewModel>(record);
            SubjectName = record.Subject?.Name ?? "Unknown";
            ObservationDefinitionName = record.ObservationDefinition?.DefinitionName ?? "Unknown";
            MetricTypeId = record.MetricTypeId;
            MetaTagNames = record.MetaTags.Select(m => m.TagName).ToList();

            return Page();
        }
    }
}