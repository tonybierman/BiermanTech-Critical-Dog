using AutoMapper;
using AutoMapper.Configuration.Annotations;
using BiermanTech.CanineHealth;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Composition;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Subjects
{
    public class DetailsModel : SubjectBasePageModel
    {
        private readonly ISubjectRecordService _subjectRecordService;

        public TrendReportViewModel WeightReport { get; private set; }
        public NutritionScienceCardViewModel NutritionPartialViewModel { get; set; }
        public List<SubjectRecordViewModel> Records { get; } = new List<SubjectRecordViewModel>();
        public IEnumerable<IGrouping<string, SubjectRecordViewModel>> GroupedRecords { get; private set; }

        public DetailsModel(
            ISubjectService subjectService,
            ISubjectRecordService subjectRecordService,
            IMapper mapper,
            IAuthorizationService authorizationService,
            ILogger<DetailsModel> logger
            )
            : base(subjectService, mapper, authorizationService, logger)
        {
            _subjectRecordService = subjectRecordService;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            if (!await RetrieveAndAuthorizeSubjectAsync("CanView"))
            {
                return NotFound();
            }

            var records = await _subjectRecordService.GetMostRecentSubjectRecordsAsync(Id);
            var viewModels = _mapper.Map<List<SubjectRecordViewModel>>(records);
            Records.AddRange(viewModels);

            GroupedRecords = Records
                .SelectMany(record => record.ObservationDefinition.ScientificDisciplines
                    .Select(discipline => new { Discipline = discipline.Name, Record = record }))
                .GroupBy(x => x.Discipline, x => x.Record);

            return Page();
        }
    }
}