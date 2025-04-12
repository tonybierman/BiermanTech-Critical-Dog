using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Pages.Admin.SubjectRecords
{
    public class EditModel : PageModel
    {
        private readonly ISubjectRecordService _recordService;

        public EditModel(ISubjectRecordService recordService)
        {
            _recordService = recordService;
        }

        [BindProperty]
        public SubjectRecordInputViewModel RecordVM { get; set; } = new SubjectRecordInputViewModel();

        public SelectList Subjects { get; set; }
        public SelectList ObservationDefinitions { get; set; }
        public SelectList MetricTypes { get; set; }
        public SelectList MetaTags { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            RecordVM = await _recordService.GetSubjectRecordViewModelByIdAsync(id);
            if (RecordVM == null)
            {
                return NotFound();
            }

            Subjects = await _recordService.GetSubjectsSelectListAsync();
            ObservationDefinitions = await _recordService.GetObservationDefinitionsSelectListAsync();
            MetricTypes = await _recordService.GetMetricTypesSelectListAsync();
            MetaTags = await _recordService.GetMetaTagsSelectListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Subjects = await _recordService.GetSubjectsSelectListAsync();
                ObservationDefinitions = await _recordService.GetObservationDefinitionsSelectListAsync();
                MetricTypes = await _recordService.GetMetricTypesSelectListAsync();
                MetaTags = await _recordService.GetMetaTagsSelectListAsync();
                return Page();
            }

            try
            {
                await _recordService.UpdateSubjectRecordAsync(RecordVM);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}