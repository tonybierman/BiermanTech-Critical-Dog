using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Helpers;
using BiermanTech.CriticalDog.Models;
using BiermanTech.CriticalDog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations
{
    public class CreateStep2Model : PageModel
    {
        private readonly ISubjectObservationService _service;
        private readonly ILogger<CreateStep2Model> _logger;
        private readonly IMapper _mapper;

        public IEnumerable<SelectListItem> SelectedListItems { get; set; }
        [BindProperty] 
        public int SelectedItem { get; set; }

        public CreateStep2Model(ISubjectObservationService service, ILogger<CreateStep2Model> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        [BindProperty]
        public CreateObservationViewModel ObservationVM { get; set; } = new CreateObservationViewModel();
        public string ObservationDefinitionName { get; set; }

        public void PopulateSelectListItems(ObservationDefinition observationDefinition)
        {
            var transformer = MetricValueTransformProviderFactory.GetProvider(observationDefinition);

            if (transformer == null)
            {
                return;
            }

            try
            {
                SelectedListItems = MetricValueTransformProviderFactory.GetProvider(observationDefinition).GetSelectListItems();
            }
            catch (NotSupportedException)
            {
                SelectedListItems = null; // Fallback to input field
            }
        }

        public async Task<IActionResult> OnGetAsync(int dogId, int? observationDefinitionId = null)
        {
            // Check if we're deep linking with observationDefinitionId
            if (observationDefinitionId.HasValue)
            {
                // Deep link: Initialize Observation from query parameters
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
                // Existing flow: Use TempData from Step 1
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

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
            var observationDefinition = await _service.GetObservationDefinitionByIdAsync(ObservationVM.ObservationDefinitionId);
            PopulateSelectListItems(observationDefinition);

            if (observationDefinition == null)
            {
                _logger.LogWarning("ObservationDefinition with ID {ObservationDefinitionId} not found.", ObservationVM.ObservationDefinitionId);

                return NotFound();
            }

            ObservationVM.ObservationDefinitionId = observationDefinition.Id;
            if (!ObservationVM.IsQualitative)
            {
                if (!ObservationVM.MetricTypeId.HasValue || !ObservationVM.MetricValue.HasValue)
                {
                    if (!ObservationVM.MetricTypeId.HasValue)
                    {
                        if (observationDefinition.MetricTypes.Any())
                        {
                            if (observationDefinition.MetricTypes.ToList().Count == 1)
                            {
                                ObservationVM.MetricTypeId = observationDefinition.MetricTypes.First().Id;
                            }
                            else 
                            {
                                ModelState.AddModelError("ObservationVM.MetricTypeId", "Please select a metric type for quantitative observations.");
                            }
                        }
                    }

                    if (!ObservationVM.MetricValue.HasValue)
                    {
                        // Try to parse MetricValue from SelectedItem if a dropdown is used
                        if (SelectedListItems?.Any() == true && SelectedItem > 0)
                        {
                            ObservationVM.MetricValue = SelectedItem;
                        }

                        // If MetricValue is still null, add an error
                        if (!ObservationVM.MetricValue.HasValue)
                        {
                            string errorMessage = SelectedListItems?.Any() == true
                                ? "Please select a valid life stage."
                                : "Please enter a value for the observation.";
                            ModelState.AddModelError("ObservationVM.MetricValue", errorMessage);
                        }
                    }

                    if (ObservationVM.MetricValue.HasValue)
                    {
                        TempData["Observation"] = System.Text.Json.JsonSerializer.Serialize(ObservationVM);

                        return RedirectToPage("CreateStep3", new { dogId });
                    }

                    ObservationVM.MetricTypes = await _service.GetMetricTypesSelectListAsync(ObservationVM.ObservationDefinitionId.Value);

                    return Page();
                }

                if (ObservationVM.MetricValue < observationDefinition.MinimumValue || ObservationVM.MetricValue > observationDefinition.MaximumValue)
                {
                    ModelState.AddModelError("ObservationVM.MetricValue", $"Value must be between {observationDefinition.MinimumValue} and {observationDefinition.MaximumValue}.");
                    ObservationVM.MetricTypes = await _service.GetMetricTypesSelectListAsync(ObservationVM.ObservationDefinitionId.Value);

                    return Page();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(ObservationVM.Note))
                {
                    ModelState.AddModelError("ObservationVM.Note", "A note is required for qualitative observations.");

                    return Page();
                }
            }

            TempData["Observation"] = System.Text.Json.JsonSerializer.Serialize(ObservationVM);

            return RedirectToPage("CreateStep3", new { dogId });
        }
    }
}