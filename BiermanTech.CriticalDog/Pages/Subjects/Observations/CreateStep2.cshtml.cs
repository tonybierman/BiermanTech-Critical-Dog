using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Helpers;
using BiermanTech.CriticalDog.Pages.Subjects.Observations.CalculationProviders;
using BiermanTech.CriticalDog.Pages.Subjects.Observations.Step2;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations
{
    public class CreateStep2Model : PageModel
    {
        private readonly ISubjectObservationService _service;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public IEnumerable<SelectListItem> SelectedListItems { get; set; }
        [BindProperty]
        public int SelectedItem { get; set; }

        [BindProperty]
        public CreateObservationViewModel ObservationVM { get; set; } = new CreateObservationViewModel();
        public string ObservationDefinitionName { get; set; }

        public CreateStep2Model(ISubjectObservationService service, ILogger<CreateStep2Model> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        public void PopulateSelectListItems(ObservationDefinition observationDefinition)
        {
            var transformer = MetricValueTransformerFactory.GetProvider(observationDefinition);

            if (transformer == null)
            {
                return;
            }

            try
            {
                SelectedListItems = MetricValueTransformerFactory.GetProvider(observationDefinition).GetSelectListItems();
            }
            catch (NotSupportedException)
            {
                SelectedListItems = null; // Fallback to input field
            }
        }

        public async Task<IActionResult> OnGetAsync(int dogId, int? observationDefinitionId = null)
        {
            if (observationDefinitionId.HasValue)
            {
                var dog = await _service.GetByIdAsync(dogId);
                if (dog == null)
                {
                    _logger.LogWarning("Dog with ID {DogId} not found.", dogId);
                    return NotFound();
                }

                var observationDefinition = await _service.GetObservationDefinitionByIdAsync(observationDefinitionId.Value);
                if (observationDefinition == null)
                {
                    _logger.LogWarning("ObservationDefinition with ID {ObservationDefinitionId} not found.", observationDefinitionId);
                    return NotFound();
                }

                ObservationDefinitionName = observationDefinition.DefinitionName;
                ObservationVM = _mapper.Map<CreateObservationViewModel>(observationDefinition);
                ObservationVM.SubjectId = dogId;
                ObservationVM.SubjectName = dog.Name ?? "Unknown";
                ObservationVM.RecordTime = DateTime.Now;

                PopulateSelectListItems(observationDefinition);
            }
            else
            {
                if (TempData["Observation"] == null)
                {
                    _logger.LogInformation("TempData['Observation'] is null. Redirecting to CreateStep1 for DogId {DogId}.", dogId);
                    return RedirectToPage("CreateStep1", new { dogId });
                }

                ObservationVM = System.Text.Json.JsonSerializer.Deserialize<CreateObservationViewModel>(TempData["Observation"].ToString())!;
                ObservationVM.SubjectId = dogId;

                var dog = await _service.GetByIdAsync(dogId);
                if (dog == null)
                {
                    _logger.LogWarning("Dog with ID {DogId} not found.", dogId);
                    return NotFound();
                }

                var observationDefinition = await _service.GetObservationDefinitionByIdAsync(ObservationVM.ObservationDefinitionId);
                if (observationDefinition == null)
                {
                    _logger.LogWarning("ObservationDefinition with ID {ObservationDefinitionId} not found.", observationDefinitionId);
                    return NotFound();
                }

                ObservationDefinitionName = observationDefinition.DefinitionName;
                ObservationVM = _mapper.Map<CreateObservationViewModel>(observationDefinition);
                ObservationVM.SubjectName = dog.Name ?? "Unknown";
                ObservationVM.RecordTime = DateTime.Now;
                TempData.Keep("Observation");

                PopulateSelectListItems(observationDefinition);
            }

            if (!ObservationVM.MetricValue.HasValue)
            {
                var calculator = MetricValueCalculatorFactory.GetProvider(ObservationDefinitionName);
                var dog = await _service.GetByIdAsync(dogId);
                if (calculator != null && calculator.CanHandle(dog, ObservationVM))
                {
                    calculator.Execute(dog, ObservationVM);
                }
            }

            return Page();
        }

        private async Task<IActionResult> ExecutePipelineAsync(int dogId, IEnumerable<ICreateStep2PostHandler> handlers)
        {
            var context = new CreateStep2PostContext
            {
                DogId = dogId,
                ObservationVM = ObservationVM,
                SelectedListItems = SelectedListItems,
                SelectedItem = SelectedItem,
                PageModel = this
            };

            foreach (var handler in handlers)
            {
                await handler.HandleAsync(context);
                if (context.Result != null)
                {
                    return context.Result;
                }
            }

            return context.Result ?? RedirectToPage("CreateStep3", new { dogId });
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
            var handlers = new List<ICreateStep2PostHandler>
            {
                new ValidateObservationDefinitionHandler(_service, _logger),
                new ProcessMetricTypeHandler(),
                new ProcessMetricValueHandler(_service),
                new SaveToTempDataHandler()
            };

            return await ExecutePipelineAsync(dogId, handlers);
        }
    }

}