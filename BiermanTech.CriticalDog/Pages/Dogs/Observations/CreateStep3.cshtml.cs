using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Models;
using BiermanTech.CriticalDog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations
{
    public class CreateStep3Model : PageModel
    {
        private readonly IDogObservationService _service;
        private readonly ILogger<CreateStep3Model> _logger;

        public CreateStep3Model(IDogObservationService service, ILogger<CreateStep3Model> logger)
        {
            _service = service;
            _logger = logger;
        }

        [BindProperty]
        public CreateObservationViewModel Observation { get; set; } = new CreateObservationViewModel();

        public string ObservationDefinitionName { get; set; }
        public string MetricTypeDescription { get; set; }

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

            ObservationDefinitionName = observationDefinition.DefinitionName;

            if (!Observation.IsQualitative && Observation.MetricTypeId.HasValue)
            {
                var metricType = await _service.GetMetricTypeByIdAsync(Observation.MetricTypeId);
                MetricTypeDescription = metricType?.Description ?? "Unknown";
            }

            Observation.MetaTags = await _service.GetMetaTagsSelectListAsync(Observation.SelectedMetaTagIds);
            TempData.Keep("Observation");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
            _logger.LogInformation("Saving observation for DogId: {DogId}, ObservationDefinitionId: {ObservationDefinitionId}, SelectedMetaTagIds: {SelectedMetaTagIds}",
                    Observation.DogId, Observation.ObservationDefinitionId, string.Join(", ", Observation.SelectedMetaTagIds ?? new List<int>()));

            var dogRecord = new DogRecord
            {
                DogId = Observation.DogId,
                MetricTypeId = Observation.MetricTypeId,
                MetricValue = Observation.MetricValue,
                Note = Observation.Note,
                RecordTime = Observation.RecordTime ?? DateTime.Now,
                CreatedBy = User.Identity?.Name ?? "Unknown"
            };

            await _service.SaveDogRecordAsync(dogRecord, Observation.SelectedMetaTagIds);
            TempData.Remove("Observation");

            return RedirectToPage("/Dogs/Index");
        }
    }
}