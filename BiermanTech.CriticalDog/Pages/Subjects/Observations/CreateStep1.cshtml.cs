using BiermanTech.CriticalDog.Pages.Subjects.Observations;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations
{
    public class CreateStep1Model : CreateStepPageModelBase
    {
        public CreateStep1Model(
            ISubjectObservationService observationService,
            IObservationWizardRouteFactory routeFactory,
            ISelectListService selectListService,
            ILogger<CreateStep1Model> logger)
            : base(observationService, routeFactory, selectListService, logger)
        {
        }

        public async Task<IActionResult> OnGetAsync(int dogId)
        {
            var notFound = await LoadDogAsync(dogId);
            if (notFound != null)
            {
                return notFound;
            }

            ObservationVM.ObservationDefinitions = await _selectListService.GetObservationDefinitionsSelectListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
            var notFound = await LoadDogAsync(dogId);
            if (notFound != null)
            {
                return notFound;
            }

            if (!ObservationVM.ObservationDefinitionId.HasValue)
            {
                ModelState.AddModelError("Observation.ObservationDefinitionId", "Please select an observation type.");
                ObservationVM.ObservationDefinitions = await _selectListService.GetObservationDefinitionsSelectListAsync();
                return Page();
            }

            var validationResult = await ValidateObservationDefinitionAsync(ObservationVM.ObservationDefinitionId.Value);
            if (validationResult != null)
            {
                return validationResult;
            }

            await EnsureObservationDefinitionName(ObservationVM.ObservationDefinitionId);
            return RedirectToNextStep("CreateStep1", dogId);
        }
    }
}