using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations.Step2GetPipeline
{
    public class HandleFlowStage : ICreateStep2GetStage
    {
        private readonly ISubjectObservationService _service;
        private readonly ILogger _logger;

        public HandleFlowStage(ISubjectObservationService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task HandleAsync(Step2GetContext context)
        {
            if (!context.ObservationDefinitionId.HasValue)
            {
                var pageModel = context.PageModel;
                if (pageModel.TempData["Observation"] == null)
                {
                    _logger.LogInformation("TempData['Observation'] is null. Redirecting to CreateStep1 for DogId {DogId}.", context.DogId);
                    context.Result = pageModel.RedirectToPage("CreateStep1", new { dogId = context.DogId });
                    return;
                }

                context.ObservationVM = JsonSerializer.Deserialize<CreateObservationViewModel>(pageModel.TempData["Observation"].ToString())!;
                context.ObservationVM.SubjectId = context.DogId;
                pageModel.TempData.Keep("Observation");
            }
        }
    }
}