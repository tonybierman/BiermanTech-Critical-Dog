using AutoMapper;
using AutoMapper.Configuration.Annotations;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations
{
    public class CreateStep1Model : PageModel
    {
        private readonly IObservationWizardRouteFactory _routeFactory;
        private readonly ISelectListService _selectListService;
        private readonly ISubjectObservationService _observationService;
        private readonly ILogger<CreateStep1Model> _logger;

        public CreateStep1Model(ISubjectObservationService observationService, IObservationWizardRouteFactory routeFactory, ISelectListService selectListService, ILogger<CreateStep1Model> logger)
        {
            _routeFactory = routeFactory;
            _selectListService = selectListService;
            _observationService = observationService;
            _logger = logger;
        }

        [BindProperty]
        public CreateObservationViewModel Observation { get; set; } = new CreateObservationViewModel();

        public async Task<IActionResult> OnGetAsync(int dogId)
        {
            var dog = await _observationService.GetByIdAsync(dogId);
            if (dog == null)
            {
                return NotFound();
            }

            Observation.SubjectId = dogId;
            Observation.SubjectName = dog.Name ?? "Unknown";
            Observation.ObservationDefinitions = await _selectListService.GetObservationDefinitionsSelectListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
            var dog = await _observationService.GetByIdAsync(dogId);
            if (dog == null)
            {
                return NotFound();

            }
            if (!Observation.ObservationDefinitionId.HasValue)
            {
                ModelState.AddModelError("Observation.ObservationDefinitionId", "Please select an observation type.");
                Observation.ObservationDefinitions = await _selectListService.GetObservationDefinitionsSelectListAsync();

                return Page();
            }

            var observationDefinition = await _observationService.GetObservationDefinitionByIdAsync(Observation.ObservationDefinitionId);
            var router = _routeFactory.GetStep2Route(observationDefinition?.Name);
            var targetPage = router.GetNextStep("CreateStep1");

            TempData["Observation"] = System.Text.Json.JsonSerializer.Serialize(Observation);

            return RedirectToPage(targetPage, new { dogId });
        }
    }
}