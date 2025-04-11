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

        public async Task<IActionResult> OnGetAsync(int dogId)
        {
            if (TempData["Observation"] == null)
            {
                return RedirectToPage("CreateStep1", new { dogId });
            }

            Observation = System.Text.Json.JsonSerializer.Deserialize<CreateObservationViewModel>(TempData["Observation"].ToString());
            Observation.DogId = dogId;
            var dog = await _service.GetDogByIdAsync(dogId);
            if (dog == null)
            {
                return NotFound();
            }

            Observation.DogName = dog.Name ?? "Unknown";
            var observationDefinition = await _service.GetObservationDefinitionByIdAsync(Observation.ObservationDefinitionId);
            if (observationDefinition == null)
            {
                return NotFound();
            }

            Observation.IsQualitative = observationDefinition.IsQualitative;
            Observation.MinValue = observationDefinition.MinimumValue;
            Observation.MaxValue = observationDefinition.MaximumValue;
            Observation.RecordTime = DateTime.Now;
            Observation.MetricTypes = await _service.GetMetricTypesSelectListAsync(Observation.ObservationDefinitionId.Value);
            TempData.Keep("Observation");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
            var observationDefinition = await _service.GetObservationDefinitionByIdAsync(Observation.ObservationDefinitionId);
            if (observationDefinition == null)
            {
                return NotFound();
            }

            Observation.IsQualitative = observationDefinition.IsQualitative;

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