using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Pages.Subjects.Observations;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.Services.Factories;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations
{
    public class CreateStep2Model : CreateStepPageModelBase
    {
        private readonly IMetricValueTransformerFactory _metricValueTransformFactory;
        private readonly IMapper _mapper;

        public IEnumerable<SelectListItem> SelectedListItems { get; set; }

        [BindProperty]
        public int SelectedItem { get; set; }

        public CreateStep2Model(
            IMetricValueTransformerFactory metricValueTransformerFactory,
            ISubjectObservationService observationService,
            ISelectListService selectListService,
            ILogger<CreateStep2Model> logger,
            IObservationWizardRouteFactory routeFactory,
            IMapper mapper)
            : base(observationService, routeFactory, selectListService, logger)
        {
            _metricValueTransformFactory = metricValueTransformerFactory;
            _mapper = mapper;
        }

        public async Task<IActionResult> OnGetAsync(int dogId, int? observationDefinitionId = null)
        {
            int definitionId = observationDefinitionId ?? (GetObservationFromTempData()?.ObservationDefinitionId ?? 0);
            if (definitionId == 0)
            {
                _logger.LogInformation("No observation definition ID provided. Redirecting to CreateStep1 for DogId {DogId}.", dogId);
                return RedirectToPage("CreateStep1", new { dogId });
            }

            var notFound = await LoadDogAsync(dogId);
            if (notFound != null)
            {
                return notFound;
            }

            var validationResult = await ValidateObservationDefinitionAsync(definitionId);
            if (validationResult != null)
            {
                return validationResult;
            }

            var observationDefinition = await _observationService.GetObservationDefinitionByIdAsync(definitionId);
            ObservationVM = _mapper.Map<CreateObservationViewModel>(observationDefinition);
            ObservationVM.SubjectId = dogId;
            ObservationVM.SubjectName = ObservationVM.SubjectName ?? "Unknown";
            ObservationVM.RecordTime = DateTime.Now;

            SelectedListItems = GetSelectListItems(observationDefinition);
            if (SelectedListItems == null)
            {
                _logger.LogWarning("Failed to populate select list items for ObservationDefinition {DefinitionId}.", definitionId);
            }

            await EnsureObservationDefinitionName(definitionId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
            var validationResult = await ValidateObservationDefinitionAsync(ObservationVM.ObservationDefinitionId);
            if (validationResult != null)
            {
                return validationResult;
            }

            var observationDefinition = await _observationService.GetObservationDefinitionByIdAsync(ObservationVM.ObservationDefinitionId);
            SelectedListItems = GetSelectListItems(observationDefinition);
            ObservationVM.ObservationDefinitionId = observationDefinition.Id;
            ObservationDefinitionName = observationDefinition.Name;

            var validationErrors = await ValidateAsync(ObservationVM, observationDefinition, SelectedItem, SelectedListItems);
            if (validationErrors.Any())
            {
                foreach (var error in validationErrors)
                {
                    ModelState.AddModelError($"ObservationVM.{error.Key}", error.Value);
                }
                ObservationVM.MetricTypes = await _selectListService.GetMetricTypesSelectListAsync(ObservationVM.ObservationDefinitionId.Value);
                return Page();
            }

            await EnsureObservationDefinitionName(ObservationVM.ObservationDefinitionId);
            return RedirectToNextStep("CreateStep2", dogId);
        }

        private async Task<Dictionary<string, string>> ValidateAsync(
            CreateObservationViewModel vm,
            ObservationDefinition definition,
            int selectedItem,
            IEnumerable<SelectListItem> selectedListItems)
        {
            var errors = new Dictionary<string, string>();

            if (!vm.MetricTypeId.HasValue && definition.MetricTypes.Any())
            {
                if (definition.MetricTypes.Count == 1)
                {
                    vm.MetricTypeId = definition.MetricTypes.First().Id;
                }
                else
                {
                    errors.Add("MetricTypeId", "Please select a metric type for quantitative observations.");
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
                errors.Add("MetricValue", $"Value must be between {definition.MinimumValue} and {definition.MaximumValue}.");
            }

            return errors;
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
                return null;
            }
        }
    }
}