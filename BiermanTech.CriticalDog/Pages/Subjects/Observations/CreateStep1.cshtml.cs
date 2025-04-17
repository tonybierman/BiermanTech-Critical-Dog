using AutoMapper;
using BiermanTech.CriticalDog.Models;
using BiermanTech.CriticalDog.Pages.Subjects.Observations.RouteProviders;
using BiermanTech.CriticalDog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations
{
    public class CreateStep1Model : PageModel
    {
        private readonly ISubjectObservationService _service;
        private readonly ILogger<CreateStep1Model> _logger;

        public CreateStep1Model(ISubjectObservationService service, ILogger<CreateStep1Model> logger)
        {
            _service = service;
            _logger = logger;
        }

        [BindProperty]
        public CreateObservationViewModel Observation { get; set; } = new CreateObservationViewModel();

        public async Task<IActionResult> OnGetAsync(int dogId)
        {
            var dog = await _service.GetByIdAsync(dogId);
            if (dog == null)
            {
                return NotFound();
            }

            Observation.SubjectId = dogId;
            Observation.SubjectName = dog.Name ?? "Unknown";
            Observation.ObservationDefinitions = await _service.GetObservationDefinitionsSelectListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
            var dog = await _service.GetByIdAsync(dogId);
            if (dog == null)
            {
                return NotFound();

            }
            if (!Observation.ObservationDefinitionId.HasValue)
            {
                ModelState.AddModelError("Observation.ObservationDefinitionId", "Please select an observation type.");
                Observation.ObservationDefinitions = await _service.GetObservationDefinitionsSelectListAsync();

                return Page();
            }

            var observationDefinition = await _service.GetObservationDefinitionByIdAsync(Observation.ObservationDefinitionId);

            // Resolve providers (injected via DI or instantiated)
            var providers = new List<IObservationRouteProvider>
            {
                new DailyCaloricIntakeObservationRouteProvider()
                // Add more providers
            };

            var typeName = observationDefinition.DefinitionName ?? "Unknown";
            var targetPage = providers.FirstOrDefault(p => p.CanHandle(dog, typeName))?.GetRoute() ?? "CreateStep2";

            TempData["Observation"] = System.Text.Json.JsonSerializer.Serialize(Observation);

            return RedirectToPage(targetPage, new { dogId });
        }
    }
}