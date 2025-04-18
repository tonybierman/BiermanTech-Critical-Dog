using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations.Step2GetPipeline
{
    public class FetchObservationDefinitionStage : ICreateStep2GetStage
    {
        private readonly ISubjectObservationService _service;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public FetchObservationDefinitionStage(ISubjectObservationService service, ILogger logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task HandleAsync(Step2GetContext context)
        {
            int? definitionId = context.ObservationDefinitionId ?? context.ObservationVM.ObservationDefinitionId;
            var observationDefinition = await _service.GetObservationDefinitionByIdAsync(definitionId);
            if (observationDefinition == null)
            {
                _logger.LogWarning("ObservationDefinition with ID {ObservationDefinitionId} not found.", definitionId);
                context.Result = new NotFoundResult();
                return;
            }

            context.ObservationDefinition = observationDefinition;
            context.ObservationDefinitionName = observationDefinition.DefinitionName;

            if (context.ObservationDefinitionId.HasValue)
            {
                context.ObservationVM = _mapper.Map<CreateObservationViewModel>(observationDefinition);
                context.ObservationVM.SubjectId = context.DogId;
                context.ObservationVM.SubjectName = context.Dog.Name ?? "Unknown";
                context.ObservationVM.RecordTime = DateTime.Now;
            }
            else
            {
                context.ObservationVM = _mapper.Map<CreateObservationViewModel>(observationDefinition);
                context.ObservationVM.SubjectName = context.Dog.Name ?? "Unknown";
                context.ObservationVM.RecordTime = DateTime.Now;
            }
        }
    }
}