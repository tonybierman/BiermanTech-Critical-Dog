using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Factories;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations
{
    public class CreateStep2Model : PageModel
    {
        private readonly IMetricValueTransformerFactory _metricValueTransformFactory;
        private readonly ISelectListService _selectListService;
        private readonly ISubjectObservationService _observationService;
        private readonly ILogger<CreateStep2Model> _logger;
        private readonly IMapper _mapper;

        public IEnumerable<SelectListItem> SelectedListItems { get; set; }
        [BindProperty]
        public int SelectedItem { get; set; }

        public CreateStep2Model(
            IMetricValueTransformerFactory metricValueTransformerFactory,
            ISubjectObservationService observationService,
            ISelectListService selectListService,
            ILogger<CreateStep2Model> logger,
            IMapper mapper)
        {
            _metricValueTransformFactory = metricValueTransformerFactory;
            _selectListService = selectListService;
            _observationService = observationService;
            _logger = logger;
            _mapper = mapper;
        }

        [BindProperty]
        public CreateObservationViewModel ObservationVM { get; set; } = new CreateObservationViewModel();
        public string ObservationDefinitionName { get; set; }

        public async Task<IActionResult> OnGetAsync(int dogId, int? observationDefinitionId = null)
        {
            int definitionId = observationDefinitionId ?? GetDefinitionIdFromTempData(dogId);
            if (definitionId == 0)
            {
                _logger.LogInformation("No observation definition ID provided. Redirecting to CreateStep1 for DogId {DogId}.", dogId);
                return RedirectToPage("CreateStep1", new { dogId });
            }

            var dog = await _observationService.GetByIdAsync(dogId);
            if (dog == null)
            {
                _logger.LogError("Dog with ID {DogId} not found.", dogId);
                return NotFound();
            }

            var observationDefinition = await _observationService.GetObservationDefinitionByIdAsync(definitionId);
            if (observationDefinition == null)
            {
                _logger.LogError("ObservationDefinition with ID {ObservationDefinitionId} not found.", definitionId);
                return NotFound();
            }

            ObservationDefinitionName = observationDefinition.Name;
            ObservationVM = _mapper.Map<CreateObservationViewModel>(observationDefinition);
            ObservationVM.SubjectId = dogId;
            ObservationVM.SubjectName = dog.Name ?? "Unknown";
            ObservationVM.RecordTime = DateTime.Now;

            SelectedListItems = GetSelectListItems(observationDefinition);
            if (SelectedListItems == null)
            {
                _logger.LogWarning("Failed to populate select list items for ObservationDefinition {DefinitionId}.", definitionId);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
            var observationDefinition = await _observationService.GetObservationDefinitionByIdAsync(ObservationVM.ObservationDefinitionId);
            if (observationDefinition == null)
            {
                _logger.LogError("ObservationDefinition with ID {ObservationDefinitionId} not found.", ObservationVM.ObservationDefinitionId);
                return NotFound();
            }

            SelectedListItems = GetSelectListItems(observationDefinition);
            ObservationVM.ObservationDefinitionId = observationDefinition.Id;

            var validationResult = await ValidateAsync(ObservationVM, observationDefinition, SelectedItem, SelectedListItems);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError($"ObservationVM.{error.Key}", error.Value);
                }
                ObservationVM.MetricTypes = await _selectListService.GetMetricTypesSelectListAsync(ObservationVM.ObservationDefinitionId.Value);
                return Page();
            }

            TempData["Observation"] = System.Text.Json.JsonSerializer.Serialize(ObservationVM);
            return RedirectToPage("CreateStep3", new { dogId });
        }

        private int GetDefinitionIdFromTempData(int dogId)
        {
            if (TempData["Observation"] == null)
            {
                return 0;
            }

            var vm = System.Text.Json.JsonSerializer.Deserialize<CreateObservationViewModel>(TempData["Observation"].ToString());
            TempData.Keep("Observation");
            return vm?.ObservationDefinitionId ?? 0;
        }

        private async Task<ValidationResult> ValidateAsync(
            CreateObservationViewModel vm,
            ObservationDefinition definition,
            int selectedItem,
            IEnumerable<SelectListItem> selectedListItems)
        {
            var result = new ValidationResult();

            if (!vm.MetricTypeId.HasValue && definition.MetricTypes.Any())
            {
                if (definition.MetricTypes.Count == 1)
                {
                    vm.MetricTypeId = definition.MetricTypes.First().Id;
                }
                else
                {
                    result.Errors.Add("MetricTypeId", "Please select a metric type for quantitative observations.");
                }
            }

            if (!vm.MetricValue.HasValue)
            {
                if (selectedListItems?.Any() == true && selectedItem > 0)
                {
                    vm.MetricValue = selectedItem;
                }
                else
                {
                    vm.MetricValue = 0m;
                }
            }

            if (vm.MetricValue.HasValue &&
                (vm.MetricValue < definition.MinimumValue || vm.MetricValue > definition.MaximumValue))
            {
                result.Errors.Add("MetricValue", $"Value must be between {definition.MinimumValue} and {definition.MaximumValue}.");
            }

            return result;
        }

        private IEnumerable<SelectListItem> GetSelectListItems(ObservationDefinition observationDefinition)
        {
            var transformer = _metricValueTransformFactory.GetProvider(observationDefinition);
            if (transformer == null)
            {
                return null;
            }

            try
            {
                return transformer.GetSelectListItems();
            }
            catch (NotSupportedException)
            {
                return null; // Fallback to input field
            }
        }

        private class ValidationResult
        {
            public Dictionary<string, string> Errors { get; } = new Dictionary<string, string>();
            public bool IsValid => Errors.Count == 0;
        }
    }
}