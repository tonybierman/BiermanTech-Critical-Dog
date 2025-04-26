using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Factories;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations
{
    public class CreateStep3Model : PageModel
    {
        private readonly ISelectListService _selectListService;
        private readonly ISubjectObservationService _observationService;
        private readonly ILogger<CreateStep3Model> _logger;
        private readonly IMetricValueTransformerFactory _metricValueTransformFactory;

        public CreateStep3Model(IMetricValueTransformerFactory metricValueTransformerFactory, ISubjectObservationService observationService, ISelectListService selectListService, ILogger<CreateStep3Model> logger)
        {
            _metricValueTransformFactory = metricValueTransformerFactory;
            _selectListService = selectListService;
            _observationService = observationService;
            _logger = logger;
        }

        [BindProperty]
        public CreateObservationViewModel Observation { get; set; } = new CreateObservationViewModel();

        public string ObservationDefinitionName { get; set; }
        public string MetricTypeDescription { get; set; }
        public IMetricValueTransformProvider MetricValueTransformer { get; private set; }

        public async Task<IActionResult> OnGetAsync(int dogId)
        {
            if (TempData["Observation"] == null)
            {
                return RedirectToPage("CreateStep1", new { dogId });
            }

            Observation = System.Text.Json.JsonSerializer.Deserialize<CreateObservationViewModel>(TempData["Observation"].ToString());
            Observation.SubjectId = dogId;
            var dog = await _observationService.GetByIdAsync(dogId);
            if (dog == null)
            {
                return NotFound();
            }

            Observation.SubjectName = dog.Name ?? "Unknown";

            var observationDefinition = await _observationService.GetObservationDefinitionByIdAsync(Observation.ObservationDefinitionId);
            if (observationDefinition == null)
            {
                return NotFound();
            }

            ObservationDefinitionName = observationDefinition.Name;

            var metricType = await _observationService.GetMetricTypeByIdAsync(Observation.MetricTypeId);
            MetricTypeDescription = metricType?.Name ?? "Unknown";

            MetricValueTransformer = _metricValueTransformFactory.GetProvider(observationDefinition);

            Observation.MetaTags = await _selectListService.GetMetaTagsSelectListAsync(Observation.SelectedMetaTagIds);
            TempData.Keep("Observation");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
            _logger.LogInformation("Saving observation for DogId: {DogId}, ObservationDefinitionId: {ObservationDefinitionId}, SelectedMetaTagIds: {SelectedMetaTagIds}",
                    Observation.SubjectId, Observation.ObservationDefinitionId, string.Join(", ", Observation.SelectedMetaTagIds ?? new List<int>()));

            var dogRecord = new SubjectRecord
            {
                ObservationDefinitionId = Observation.ObservationDefinitionId.Value,
                SubjectId = Observation.SubjectId,
                MetricTypeId = Observation.MetricTypeId,
                MetricValue = Observation.MetricValue,
                Note = Observation.Note,
                RecordTime = Observation.RecordTime ?? DateTime.Now,
                CreatedBy = User.Identity?.Name ?? "Unknown"
            };

            var result = await ServiceHelper.ExecuteAsyncOperation(
                () => _observationService.SaveSubjectRecordAsync(dogRecord, Observation.SelectedMetaTagIds),
                TempData,
                _logger
            );

            TempData.Remove("Observation");

            return RedirectToPage("/Subjects/Index");
        }
    }
}