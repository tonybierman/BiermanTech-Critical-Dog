using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Pages.Admin.SubjectRecords
{
    public class CreateModel : PageModel
    {
        private readonly ISubjectRecordService _recordService;
        private readonly ISelectListService _selectListService;

        public CreateModel(ISubjectRecordService recordService, ISelectListService selectListService)
        {
            _recordService = recordService;
            _selectListService = selectListService;
        }

        [BindProperty]
        public SubjectRecordInputViewModel RecordVM { get; set; } = new SubjectRecordInputViewModel();

        public SelectList Subjects { get; set; }
        public SelectList ObservationDefinitions { get; set; }
        public SelectList MetricTypes { get; set; }
        public SelectList MetaTags { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await EnsureSelectLists();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await EnsureSelectLists();
                return Page();
            }

            await _recordService.CreateSubjectRecordAsync(RecordVM);
            return RedirectToPage("./Index");
        }

        private async Task EnsureSelectLists()
        {
            Subjects = await _selectListService.GetSubjectsSelectListAsync();
            ObservationDefinitions = await _selectListService.GetObservationDefinitionsSelectListAsync();
            MetricTypes = await _selectListService.GetMetricTypesSelectListAsync();
            MetaTags = await _selectListService.GetMetaTagsSelectListAsync();
        }
    }
}