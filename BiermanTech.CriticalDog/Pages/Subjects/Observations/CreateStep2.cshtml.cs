using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Helpers;
using BiermanTech.CriticalDog.Pages.Dogs.Observations.Step2GetPipeline;
using BiermanTech.CriticalDog.Pages.Subjects.Observations.Step2PostPipeline;
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
            var stages = new List<ICreateStep2GetStage>
            {
                new ValidateDogStage(_service, _logger),
                new HandleFlowStage(_service, _logger),
                new FetchObservationDefinitionStage(_service, _logger, _mapper),
                new PopulateViewModelStage(this),
                new CalculateMetricValueStage(_service)
            };

            return await ExecuteGetPipelineAsync(dogId, observationDefinitionId, stages);
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
            var stages = new List<ICreateStep2PostStage>
            {
                new ValidateObservationDefinitionStage(_service, _logger),
                new ProcessMetricTypeStage(),
                new ProcessMetricValueStage(_service),
                new SaveToTempDataStage()
            };

            return await ExecutePostPipelineAsync(dogId, stages);
        }

        private async Task<IActionResult> ExecuteGetPipelineAsync(int dogId, int? observationDefinitionId, IEnumerable<ICreateStep2GetStage> stages)
        {
            var context = new Step2GetContext
            {
                DogId = dogId,
                ObservationDefinitionId = observationDefinitionId,
                ObservationVM = ObservationVM,
                PageModel = this
            };

            foreach (var handler in stages)
            {
                await handler.HandleAsync(context);
                if (context.Result != null)
                {
                    return context.Result;
                }
            }

            return context.Result ?? Page();
        }

        private async Task<IActionResult> ExecutePostPipelineAsync(int dogId, IEnumerable<ICreateStep2PostStage> stages)
        {
            var context = new CreateStep2PostContext
            {
                DogId = dogId,
                ObservationVM = ObservationVM,
                SelectedListItems = SelectedListItems,
                SelectedItem = SelectedItem,
                PageModel = this
            };

            foreach (var handler in stages)
            {
                await handler.HandleAsync(context);
                if (context.Result != null)
                {
                    return context.Result;
                }
            }

            return context.Result ?? RedirectToPage("CreateStep3", new { dogId });
        }
    }
}