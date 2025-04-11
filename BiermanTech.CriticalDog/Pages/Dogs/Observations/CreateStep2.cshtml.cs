using BiermanTech.CriticalDog.Models;
using BiermanTech.CriticalDog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations
{
    public class CreateStep2Model : PageModel
    {
        private readonly IDogObservationService _service;
        private readonly ILogger<CreateStep2Model> _logger;

        public CreateStep2Model(IDogObservationService service, ILogger<CreateStep2Model> logger)
        {
            _service = service;
            _logger = logger;
        }

        [BindProperty]
        public CreateObservationViewModel Observation { get; set; } = new CreateObservationViewModel();

        public async Task<IActionResult> OnGetAsync(int dogId, int? observationDefinitionId = null)
        {
            // Check if we're deep linking with observationDefinitionId
            if (observationDefinitionId.HasValue)
            {
                // Deep link: Initialize Observation from query parameters
                var dog = await _service.GetDogByIdAsync(dogId);
                if (dog == null)
                {
                    _logger.LogWarning("Dog with ID {DogId} not found.", dogId);
                    return NotFound();
                }

                var observationDefinition = await _service.GetObservationDefinitionByIdAsync(observationDefinitionId.Value);
                if (observationDefinition == null)
                {
                    _logger.LogWarning("ObservationDefinition with ID {ObservationDefinitionId} not found.", observationDefinitionId);
                    return NotFound();
                }

                Observation.DogId = dogId;
                Observation.DogName = dog.Name ?? "Unknown";
                Observation.ObservationDefinitionId = observationDefinitionId;
                Observation.IsQualitative = observationDefinition.IsQualitative;
                Observation.MinValue = observationDefinition.MinimumValue;
                Observation.MaxValue = observationDefinition.MaximumValue;
                Observation.RecordTime = DateTime.Now;
                Observation.MetricTypes = await _service.GetMetricTypesSelectListAsync(observationDefinitionId.Value);
            }
            else
            {
                // Existing flow: Use TempData from Step 1
                if (TempData["Observation"] == null)
                {
                    _logger.LogInformation("TempData['Observation'] is null. Redirecting to CreateStep1 for DogId {DogId}.", dogId);
                    return RedirectToPage("CreateStep1", new { dogId });
                }

                Observation = System.Text.Json.JsonSerializer.Deserialize<CreateObservationViewModel>(TempData["Observation"].ToString())!;
                Observation.DogId = dogId;

                var dog = await _service.GetDogByIdAsync(dogId);
                if (dog == null)
                {
                    _logger.LogWarning("Dog with ID {DogId} not found.", dogId);
                    return NotFound();
                }

                Observation.DogName = dog.Name ?? "Unknown";
                var observationDefinition = await _service.GetObservationDefinitionByIdAsync(Observation.ObservationDefinitionId);
                if (observationDefinition == null)
                {
                    _logger.LogWarning("ObservationDefinition with ID {ObservationDefinitionId} not found.", Observation.ObservationDefinitionId);
                    return NotFound();
                }

                Observation.IsQualitative = observationDefinition.IsQualitative;
                Observation.MinValue = observationDefinition.MinimumValue;
                Observation.MaxValue = observationDefinition.MaximumValue;
                Observation.RecordTime = DateTime.Now;
                Observation.MetricTypes = await _service.GetMetricTypesSelectListAsync(Observation.ObservationDefinitionId.Value);
                TempData.Keep("Observation");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
            var observationDefinition = await _service.GetObservationDefinitionByIdAsync(Observation.ObservationDefinitionId);
            if (observationDefinition == null)
            {
                _logger.LogWarning("ObservationDefinition with ID {ObservationDefinitionId} not found.", Observation.ObservationDefinitionId);
                return NotFound();
            }

            Observation.IsQualitative = observationDefinition.IsQualitative;
            Observation.MinValue = observationDefinition.MinimumValue;
            Observation.MaxValue = observationDefinition.MaximumValue;

            if (!Observation.IsQualitative)
            {
                if (!Observation.MetricTypeId.HasValue || !Observation.MetricValue.HasValue)
                {
                    if (!Observation.MetricTypeId.HasValue)
                    {
                        ModelState.AddModelError("Observation.MetricTypeId", "Please select a metric type for quantitative observations.");
                    }

                    if (!Observation.MetricValue.HasValue)
                    {
                        ModelState.AddModelError("Observation.MetricValue", "Please enter a value for quantitative observations.");
                    }

                    Observation.MetricTypes = await _service.GetMetricTypesSelectListAsync(Observation.ObservationDefinitionId.Value);
                    return Page();
                }

                if (Observation.MetricValue < observationDefinition.MinimumValue || Observation.MetricValue > observationDefinition.MaximumValue)
                {
                    ModelState.AddModelError("Observation.MetricValue", $"Value must be between {observationDefinition.MinimumValue} and {observationDefinition.MaximumValue}.");
                    Observation.MetricTypes = await _service.GetMetricTypesSelectListAsync(Observation.ObservationDefinitionId.Value);
                    return Page();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Observation.Note))
                {
                    ModelState.AddModelError("Observation.Note", "A note is required for qualitative observations.");
                    return Page();
                }
            }

            TempData["Observation"] = System.Text.Json.JsonSerializer.Serialize(Observation);
            return RedirectToPage("CreateStep3", new { dogId });
        }
    }
}