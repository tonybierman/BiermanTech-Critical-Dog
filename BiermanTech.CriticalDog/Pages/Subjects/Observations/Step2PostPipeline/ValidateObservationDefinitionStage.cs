using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Pages.Dogs.Observations;
using BiermanTech.CriticalDog.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.Step2PostPipeline
{
    public class ValidateObservationDefinitionStage : ICreateStep2PostStage
    {
        private readonly ISubjectObservationService _service;
        private readonly ILogger _logger;

        public ValidateObservationDefinitionStage(ISubjectObservationService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task HandleAsync(CreateStep2PostContext context)
        {
            var observationDefinition = await _service.GetObservationDefinitionByIdAsync(context.ObservationVM.ObservationDefinitionId);
            if (observationDefinition == null)
            {
                _logger.LogWarning("ObservationDefinition with ID {ObservationDefinitionId} not found.", context.ObservationVM.ObservationDefinitionId);
                context.Result = new NotFoundResult();
                return;
            }

            context.ObservationDefinition = observationDefinition;
            context.ObservationVM.ObservationDefinitionId = observationDefinition.Id;

            var pageModel = (CreateStep2Model)context.PageModel;
            pageModel.PopulateSelectListItems(observationDefinition);
            context.SelectedListItems = pageModel.SelectedListItems;
        }
    }
}