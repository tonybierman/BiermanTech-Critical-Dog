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

        public CreateModel(AppDbContext context)
        {
            _context = context;
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

            await LoadDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!Observation.ObservationDefinitionId.HasValue)
            {
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
                    ModelState.AddModelError("Observation.MetricTypeId", "Please select a metric type for quantitative observations.");
                    ModelState.AddModelError("Observation.MetricValue", "Please enter a value for quantitative observations.");
                    await LoadDropdownsAsync();
                    return Page();
                }

                if (Observation.MetricValue < observationDefinition.MinimumValue || Observation.MetricValue > observationDefinition.MaximumValue)
                {
                    ModelState.AddModelError("Observation.MetricValue", $"Value must be between {observationDefinition.MinimumValue} and {observationDefinition.MaximumValue}.");
                    await LoadDropdownsAsync();
                    return Page();
                }
            }

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

            if (Observation.SelectedMetaTagIds.Any())
            {
                var metaTags = await _context.MetaTags
                    .Where(mt => Observation.SelectedMetaTagIds.Contains(mt.Id))
                    .ToListAsync();
                foreach (var metaTag in metaTags)
                {
                    dogRecord.MetaTags.Add(metaTag);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Dogs/Index");
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
                "Text");
        }
    }
}