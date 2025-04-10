using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(AppDbContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public CreateObservationViewModel Observation { get; set; } = new CreateObservationViewModel();

        public async Task<IActionResult> OnGetAsync(int dogId)
        {
            var dog = await _context.Dogs.FindAsync(dogId);
            if (dog == null)
            {
                return NotFound();
            }

            Observation.DogId = dogId;
            Observation.DogName = dog.Name ?? "Unknown";
            Observation.RecordTime = DateTime.Now;

            await LoadDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetUpdateObservationDetailsAsync(int dogId, int observationDefinitionId)
        {
            _logger.LogInformation("OnGetUpdateObservationDetailsAsync called with dogId: {DogId}, observationDefinitionId: {ObservationDefinitionId}", dogId, observationDefinitionId);

            var observationDefinition = await _context.ObservationDefinitions
                .FirstOrDefaultAsync(od => od.Id == observationDefinitionId);
            if (observationDefinition == null)
            {
                return NotFound();
            }

            var metricTypes = await _context.MetricTypes
                .Where(mt => mt.ObservationDefinitionId == observationDefinitionId && mt.IsActive == true)
                .Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.Description })
                .ToListAsync();

            var metricTypesHtml = "<option value=''>-- Select Unit --</option>";
            foreach (var item in metricTypes)
            {
                metricTypesHtml += $"<option value='{item.Value}' {(item.Value == Observation.MetricTypeId?.ToString() ? "selected" : "")}>{item.Text}</option>";
            }

            return new JsonResult(new
            {
                isQualitative = observationDefinition.IsQualitative,
                metricTypes = metricTypesHtml
            });
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
            _logger.LogInformation("OnPostAsync called. ObservationDefinitionId: {ObservationDefinitionId}, MetricTypeId: {MetricTypeId}, MetricValue: {MetricValue}, SelectedMetaTagIds: {SelectedMetaTagIds}",
                Observation.ObservationDefinitionId, Observation.MetricTypeId, Observation.MetricValue, string.Join(", ", Observation.SelectedMetaTagIds ?? new List<int>()));

            if (!Observation.ObservationDefinitionId.HasValue)
            {
                ModelState.AddModelError("Observation.ObservationDefinitionId", "Please select an observation type.");
                await LoadDropdownsAsync();
                return Page();
            }

            var observationDefinition = await _context.ObservationDefinitions
                .FirstOrDefaultAsync(od => od.Id == Observation.ObservationDefinitionId);
            if (observationDefinition == null)
            {
                return NotFound();
            }

            Observation.IsQualitative = observationDefinition.IsQualitative;

            if (!Observation.IsQualitative)
            {
                if (!Observation.MetricTypeId.HasValue || !Observation.MetricValue.HasValue)
                {
                    if (!Observation.MetricTypeId.HasValue)
                    {
                        ModelState.AddModelError("Observation.MetricTypeId", "Please select a metric type for quantitative observations.");
                    }
                    if (!Observation.MetricValue.HasValue)
                    {
                        ModelState.AddModelError("Observation.MetricValue", "Please enter a value for quantitative observations.");
                    }
                    await LoadDropdownsAsync();
                    return Page();
                }

                if (Observation.MetricValue < observationDefinition.MinimumValue || Observation.MetricValue > observationDefinition.MaximumValue    )
                {
                    ModelState.AddModelError("Observation.MetricValue", $"Value must be between {observationDefinition.MinimumValue} and {observationDefinition.MaximumValue}.");
                    await LoadDropdownsAsync();
                    return Page();
                }
            }

            if (ModelState.IsValid)
            {
                _logger.LogInformation("ModelState is valid. Saving observation...");
                var dogRecord = new DogRecord
                {
                    DogId = Observation.DogId,
                    MetricTypeId = Observation.MetricTypeId,
                    MetricValue = Observation.MetricValue,
                    Note = Observation.Note,
                    RecordTime = Observation.RecordTime ?? DateTime.Now,
                    CreatedBy = User.Identity?.Name ?? "Unknown"
                };

                _context.DogRecords.Add(dogRecord);
                await _context.SaveChangesAsync();

                if (Observation.SelectedMetaTagIds?.Any() == true)
                {
                    _logger.LogInformation("Saving meta tags: {MetaTagIds}", string.Join(", ", Observation.SelectedMetaTagIds));
                    foreach (var tagId in Observation.SelectedMetaTagIds)
                    {
                        var metaTagExists = await _context.MetaTags.AnyAsync(mt => mt.Id == tagId);
                        if (metaTagExists)
                        {
                            dogRecord.MetaTags.Add(new MetaTag
                            {
                                Id = tagId
                            });
                        }
                        else
                        {
                            _logger.LogWarning("MetaTag with Id {TagId} not found.", tagId);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Observation saved successfully. Redirecting to /Dogs/Index.");
                return RedirectToPage("/Dogs/Index");
            }

            _logger.LogInformation("ModelState is invalid. Errors: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            await LoadDropdownsAsync();
            return Page();
        }

        private async Task LoadDropdownsAsync()
        {
            Observation.ObservationDefinitions = new SelectList(
                await _context.ObservationDefinitions
                    .Where(od => od.IsActive == true)
                    .Select(od => new SelectListItem { Value = od.Id.ToString(), Text = od.DefinitionName })
                    .ToListAsync(),
                "Value",
                "Text",
                Observation.ObservationDefinitionId?.ToString());

            if (Observation.ObservationDefinitionId.HasValue)
            {
                var observationDefinition = await _context.ObservationDefinitions
                    .FirstOrDefaultAsync(od => od.Id == Observation.ObservationDefinitionId);
                Observation.IsQualitative = observationDefinition?.IsQualitative ?? false;

                Observation.MetricTypes = new SelectList(
                    await _context.MetricTypes
                        .Where(mt => mt.ObservationDefinitionId == Observation.ObservationDefinitionId && mt.IsActive == true)
                        .Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.Description })
                        .ToListAsync(),
                    "Value",
                    "Text",
                    Observation.MetricTypeId?.ToString());
            }
            else
            {
                Observation.MetricTypes = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            Observation.MetaTags = new SelectList(
                await _context.MetaTags
                    .Where(mt => mt.IsActive == true)
                    .Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.TagName })
                    .ToListAsync(),
                "Value",
                "Text",
                Observation.SelectedMetaTagIds);
        }
    }
}