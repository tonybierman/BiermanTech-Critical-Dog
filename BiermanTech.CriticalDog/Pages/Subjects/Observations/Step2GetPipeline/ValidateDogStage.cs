using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations.Step2GetPipeline
{
    public class ValidateDogStage : ICreateStep2GetStage
    {
        private readonly ISubjectObservationService _service;
        private readonly ILogger _logger;

        public ValidateDogStage(ISubjectObservationService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task HandleAsync(Step2GetContext context)
        {
            var dog = await _service.GetByIdAsync(context.DogId);
            if (dog == null)
            {
                _logger.LogWarning("Dog with ID {DogId} not found.", context.DogId);
                context.Result = new NotFoundResult();
                return;
            }

            context.Dog = dog;
        }
    }
}