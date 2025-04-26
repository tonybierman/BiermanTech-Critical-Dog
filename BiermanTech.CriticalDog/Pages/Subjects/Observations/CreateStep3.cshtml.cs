using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Pages.Subjects.Observations;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.Services.Factories;
using BiermanTech.CriticalDog.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations
{
    public class CreateStep3Model : CreateStepPageModelBase
    {
        private readonly IMetricValueTransformerFactory _metricValueTransformFactory;

        public string MetricTypeDescription { get; set; }
        public IMetricValueTransformProvider MetricValueTransformer { get; private set; }

        public CreateStep3Model(
            IMetricValueTransformerFactory metricValueTransformerFactory,
            ISubjectObservationService observationService,
            ISelectListService selectListService,
            IObservationWizardRouteFactory routeFactory,
            ILogger<CreateStep3Model> logger)
            : base(observationService, routeFactory, selectListService, logger)
        {
            _metricValueTransformFactory = metricValueTransformerFactory;
        }

        public async Task<IActionResult> OnGetAsync(int dogId)
        {
            var vm = GetObservationFromTempData();
            if (vm == null)
            {
                return RedirectToPage("/Subjects/Index", new { dogId });
            }

            ObservationVM = vm;

            var notFound = await LoadDogAsync(dogId);
            if (notFound != null)
            {
                return notFound;
            }

            var validationResult = await ValidateObservationDefinitionAsync(ObservationVM.ObservationDefinitionId);
            if (validationResult != null)
            {
                return validationResult;
            }

            var observationDefinition = await _observationService.GetObservationDefinitionByIdAsync(ObservationVM.ObservationDefinitionId);
            var metricType = await _observationService.GetMetricTypeByIdAsync(ObservationVM.MetricTypeId);
            MetricTypeDescription = metricType?.Name ?? "Unknown";
            MetricValueTransformer = _metricValueTransformFactory.GetProvider(observationDefinition);
            ObservationVM.MetaTags = await _selectListService.GetMetaTagsSelectListAsync(ObservationVM.SelectedMetaTagIds);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
            _logger.LogInformation("Saving observation for DogId: {DogId}, ObservationDefinitionId: {ObservationDefinitionId}, SelectedMetaTagIds: {SelectedMetaTagIds}",
                ObservationVM.SubjectId, ObservationVM.ObservationDefinitionId, string.Join(", ", ObservationVM.SelectedMetaTagIds ?? new List<int>()));

            var dogRecord = new SubjectRecord
            {
                ObservationDefinitionId = ObservationVM.ObservationDefinitionId.Value,
                SubjectId = ObservationVM.SubjectId,
                MetricTypeId = ObservationVM.MetricTypeId,
                MetricValue = ObservationVM.MetricValue,
                Note = ObservationVM.Note,
                RecordTime = ObservationVM.RecordTime ?? DateTime.Now,
                CreatedBy = User.Identity?.Name ?? "Unknown"
            };

            var result = await ServiceHelper.ExecuteAsyncOperation(
                () => _observationService.SaveSubjectRecordAsync(dogRecord, ObservationVM.SelectedMetaTagIds),
                TempData,
                _logger
            );

            await EnsureObservationDefinitionName(ObservationVM.ObservationDefinitionId);
            return RedirectToOutStep();
        }
    }
}