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
    public class DisciplinesModel : SubjectBasePageModel
    {
        private readonly IDisciplineCardFactory _cardFactory;
        private readonly IEnergyCalculationService _energyCalculationService;
        private readonly IObservationAnalyticsProvider _analyticsProvider;
        private readonly ISubjectRecordService _subjectRecordService;
        [BindProperty(SupportsGet = true)]
        public string? Slug { get; set; }

        public TrendReportViewModel WeightReport { get; private set; }
        public NutritionScienceCardViewModel NutritionPartialViewModel { get; set; }
        public List<SubjectRecordViewModel> Records { get; } = new List<SubjectRecordViewModel>();
        public IDisciplineCardProvider CardProvider { get; private set; }

        public DisciplinesModel(
            ISubjectService subjectService,
            ISubjectRecordService subjectRecordService,
            IEnergyCalculationService energyCalculationService,
            IMapper mapper,
            IAuthorizationService authorizationService,
            ILogger<DetailsModel> logger,
            IObservationAnalyticsProvider analyticsProvider,
            IDisciplineCardFactory cardFactory)
            : base(subjectService, mapper, authorizationService, logger)
        {
            _cardFactory = cardFactory;
            _energyCalculationService = energyCalculationService;
            _analyticsProvider = analyticsProvider;
            _subjectRecordService = subjectRecordService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (Slug == null || !await RetrieveAndAuthorizeSubjectAsync("CanView"))
            {
                return NotFound();
            }

            try
            {
                CardProvider = _cardFactory.CreateProvider(this.Id, Slug);
                if (CardProvider != null)
                {
                    await CardProvider.Init();
                }
            }
            catch (ArgumentException)
            {
                return NotFound();
            }

            var records = await _subjectRecordService.GetMostRecentSubjectRecordsByDisciplineAsync(this.Id, Slug);
            var viewModels = _mapper.Map<List<SubjectRecordViewModel>>(records);
            Records.AddRange(viewModels);

            return Page();
        }
    }
}