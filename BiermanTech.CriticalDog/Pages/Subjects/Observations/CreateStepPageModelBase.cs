using BiermanTech.CriticalDog.Services.Factories;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations
{
    public abstract class CreateStepPageModelBase : PageModel
    {
        protected readonly ISubjectObservationService _observationService;
        protected readonly IObservationWizardRouteFactory _routeFactory;
        protected readonly ISelectListService _selectListService;
        protected readonly ILogger _logger;

        [BindProperty]
        public CreateObservationViewModel ObservationVM { get; set; } = new CreateObservationViewModel();
        public string? ObservationDefinitionName { get; protected set; }

        protected CreateStepPageModelBase(
            ISubjectObservationService observationService,
            IObservationWizardRouteFactory routeFactory,
            ISelectListService selectListService,
            ILogger logger)
        {
            _observationService = observationService;
            _routeFactory = routeFactory;
            _selectListService = selectListService;
            _logger = logger;
        }

        protected async Task<IActionResult> LoadDogAsync(int dogId)
        {
            var dog = await _observationService.GetByIdAsync(dogId);
            if (dog == null)
            {
                _logger.LogError("Dog with ID {DogId} not found.", dogId);
                return NotFound();
            }
            ObservationVM.SubjectId = dogId;
            ObservationVM.SubjectName = dog.Name ?? "Unknown";
            return null;
        }

        protected CreateObservationViewModel GetObservationFromTempData()
        {
            if (TempData["Observation"] == null)
            {
                return null;
            }
            var vm = JsonSerializer.Deserialize<CreateObservationViewModel>(TempData["Observation"].ToString());
            TempData.Keep("Observation");
            return vm;
        }

        protected void SaveObservationToTempData()
        {
            TempData["Observation"] = JsonSerializer.Serialize(ObservationVM);
        }

        protected async Task<IActionResult> ValidateObservationDefinitionAsync(int? definitionId)
        {
            if (definitionId == null)
            {
                return NotFound();
            }

            var observationDefinition = await _observationService.GetObservationDefinitionByIdAsync(definitionId.Value);
            if (observationDefinition == null)
            {
                _logger.LogError("ObservationDefinition with ID {ObservationDefinitionId} not found.", definitionId);
                return NotFound();
            }

            ObservationDefinitionName = observationDefinition.Name;
            return null;
        }

        protected async Task EnsureObservationDefinitionName(int? definitionId)
        {
            if (!string.IsNullOrEmpty(ObservationDefinitionName) || definitionId == null)
            {
                return;
            }

            var observationDefinition = await _observationService.GetObservationDefinitionByIdAsync(definitionId.Value);
            if (observationDefinition == null || string.IsNullOrEmpty(observationDefinition.Name))
            {
                throw new InvalidOperationException($"Observation definition with ID {definitionId} not found or has an invalid name.");
            }

            ObservationDefinitionName = observationDefinition.Name;
        }

        protected IActionResult RedirectToNextStep(string currentStep, int dogId)
        {
            var router = _routeFactory.GetRouter(ObservationDefinitionName);
            var targetPage = router.GetNextStep(currentStep);
            SaveObservationToTempData();
            return RedirectToPage(targetPage, new { dogId });
        }

        protected IActionResult RedirectToOutStep()
        {
            var router = _routeFactory.GetRouter(ObservationDefinitionName);
            var targetPage = router.GetOutStep();
            TempData.Remove("Observation");
            return RedirectToPage(targetPage);
        }
    }
}