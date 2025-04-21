using AutoMapper;
using AutoMapper.Configuration.Annotations;
using BiermanTech.CanineHealth;
using BiermanTech.CriticalDog.Analytics;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services;
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
        private readonly IObservationAnalyticsProvider _analyticsProvider;
        private readonly ISubjectRecordService _subjectRecordService;

        public NutritionSciencePartialViewModel NutritionPartialViewModel { get; set; }

        public DetailsModel(
            ISubjectService subjectService,
            ISubjectRecordService subjectRecordService,
            IMapper mapper,
            IAuthorizationService authorizationService,
            ILogger<DetailsModel> logger,
            IObservationAnalyticsProvider analyticsProvider)
            : base(subjectService, mapper, authorizationService, logger)
        {
            _analyticsProvider = analyticsProvider;
            _subjectRecordService = subjectRecordService;
        }

        public TrendReport WeightReport { get; private set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!await RetrieveAndAuthorizeSubjectAsync(id, "CanView"))
            {
                return NotFound();
            }

            WeightReport = await _analyticsProvider.GetObservationChangeReportAsync(id, "WeighIn");

            var weightRecord = await _subjectRecordService.GetMostRecentSubjectRecordAsync(id, "WeighIn");
            var lifeStageRecord = await _subjectRecordService.GetMostRecentSubjectRecordAsync(id, "CanineLifeStageFactor");

            NutritionPartialViewModel = new NutritionSciencePartialViewModel()
            {
                WeightRecord = weightRecord,
                LifestageRecord = lifeStageRecord,
                WeightReport = WeightReport,
                AnalyticPartialVM = new AnalyticsReportPartialViewModel() { Report = WeightReport }
            };

            return Page();
        }
    }
}