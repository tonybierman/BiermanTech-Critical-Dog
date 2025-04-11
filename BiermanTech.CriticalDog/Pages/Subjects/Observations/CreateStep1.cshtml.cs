using AutoMapper;
using BiermanTech.CriticalDog.Models;
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
            if (!Observation.ObservationDefinitionId.HasValue)
            {
                ModelState.AddModelError("Observation.ObservationDefinitionId", "Please select an observation type.");
                Observation.ObservationDefinitions = await _service.GetObservationDefinitionsSelectListAsync();

                return Page();
            }

            TempData["Observation"] = System.Text.Json.JsonSerializer.Serialize(Observation);

            return RedirectToPage("CreateStep2", new { dogId });
        }
    }
}