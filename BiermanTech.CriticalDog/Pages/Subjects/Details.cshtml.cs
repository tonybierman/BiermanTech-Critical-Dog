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
        private readonly IEnergyCalculationService _energyCalculationService;
        private readonly IObservationAnalyticsProvider _analyticsProvider;
        private readonly ISubjectRecordService _subjectRecordService;

        public TrendReportViewModel WeightReport { get; private set; }
        public NutritionScienceCardViewModel NutritionPartialViewModel { get; set; }
        public List<SubjectRecordViewModel> Records { get; } = new List<SubjectRecordViewModel>();
        public IEnumerable<IGrouping<string, SubjectRecordViewModel>> GroupedRecords { get; private set; }

        public DetailsModel(
            ISubjectService subjectService,
            ISubjectRecordService subjectRecordService,
            IEnergyCalculationService energyCalculationService,
            IMapper mapper,
            IAuthorizationService authorizationService,
            ILogger<DetailsModel> logger,
            IObservationAnalyticsProvider analyticsProvider)
            : base(subjectService, mapper, authorizationService, logger)
        {
            _energyCalculationService = energyCalculationService;
            _analyticsProvider = analyticsProvider;
            _subjectRecordService = subjectRecordService;
        }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!await RetrieveAndAuthorizeSubjectAsync(id, "CanView"))
            {
                return NotFound();
            }

            var records = await _subjectRecordService.GetMostRecentSubjectRecordsAsync(id);
            WeightReport = await _analyticsProvider.GetObservationChangeReportAsync(id, "WeighIn");
            NutritionPartialViewModel = new NutritionScienceCardViewModel(_energyCalculationService)
            {
                IdealWeightRecord = records?.Where(r => r.ObservationDefinition.Name == "IdealWeight")?.FirstOrDefault(),
                WeightRecord = records?.Where(r => r.ObservationDefinition.Name == "WeighIn")?.FirstOrDefault(),
                LifestageRecord = records?.Where(r => r.ObservationDefinition.Name == "CanineLifeStageFactor")?.FirstOrDefault(),
                WeightReport = WeightReport,
                AnalyticPartialVM = new AnalyticsReportPartialViewModel() { Report = WeightReport }
            };
            await NutritionPartialViewModel.Init();

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