using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Helpers;
using BiermanTech.CriticalDog.Helpers.BiermanTech.CriticalDog.Helpers;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations
{
    public class CreateStep3Model : PageModel
    {
        private readonly ISubjectObservationService _service;
        private readonly ILogger<CreateStep3Model> _logger;

        public CreateStep3Model(ISubjectObservationService service, ILogger<CreateStep3Model> logger)
        {
            _service = service;
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
            var dog = await _service.GetByIdAsync(dogId);
            if (dog == null)
            {
                return NotFound();
            }

            Observation.SubjectName = dog.Name ?? "Unknown";

            var observationDefinition = await _service.GetObservationDefinitionByIdAsync(Observation.ObservationDefinitionId);
            if (observationDefinition == null)
            {
                return NotFound();
            }

            ObservationDefinitionName = observationDefinition.DefinitionName;

            var metricType = await _service.GetMetricTypeByIdAsync(Observation.MetricTypeId);
            MetricTypeDescription = metricType?.Description ?? "Unknown";

            MetricValueTransformer = MetricValueTransformerFactory.GetProvider(observationDefinition);

            Observation.MetaTags = await _service.GetMetaTagsSelectListAsync(Observation.SelectedMetaTagIds);
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

            try
            {
                int rows = await _service.SaveSubjectRecordAsync(dogRecord, Observation.SelectedMetaTagIds);

                if (rows > 0)
                {
                    TempData[Constants.AlertSuccess] = $"Record saved.  {rows} rows affected.";
                }
                else
                {
                    TempData[Constants.AlertWarning] = $"Record not saved.  {rows} rows affected.";
                }
            }            
            catch (Exception ex)
            {
                TempData[Constants.AlertDanger] = ex.GetAllExceptionMessages();
                _logger.LogError(ex, ex.GetAllExceptionMessages());
            }


            TempData.Remove("Observation");

            return RedirectToPage("/Subjects/Index");
        }
    }
}