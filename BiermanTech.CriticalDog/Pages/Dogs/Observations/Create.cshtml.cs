using BiermanTech.CriticalDog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
        public int DogId { get; set; }

        public string DogName { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Please select an observation type.")]
        public int? ObservationDefinitionId { get; set; }

        [BindProperty]
        public int? MetricTypeId { get; set; }

        [BindProperty]
        public decimal? MetricValue { get; set; }

        [BindProperty]
        public string? Note { get; set; }

        [BindProperty]
        public DateTime? RecordTime { get; set; }

        [BindProperty]
        public List<int> SelectedMetaTagIds { get; set; } = new List<int>();

        public bool IsQualitative { get; set; }

        public SelectList ObservationDefinitions { get; set; } = null!;
        public SelectList MetricTypes { get; set; } = null!;
        public SelectList MetaTags { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int dogId)
        {
            var dog = await _context.Dogs.FindAsync(dogId);
            if (dog == null)
            {
                return NotFound();
            }

            DogId = dogId;
            DogName = dog.Name ?? "Unknown";

            await LoadDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ObservationDefinitionId.HasValue)
            {
                await LoadDropdownsAsync();
                return Page();
            }

            var observationDefinition = await _context.ObservationDefinitions
                .FirstOrDefaultAsync(od => od.Id == ObservationDefinitionId);
            if (observationDefinition == null)
            {
                return NotFound();
            }

            IsQualitative = observationDefinition.IsQualitative;

            if (!IsQualitative)
            {
                if (!MetricTypeId.HasValue || !MetricValue.HasValue)
                {
                    ModelState.AddModelError("MetricTypeId", "Please select a metric type for quantitative observations.");
                    ModelState.AddModelError("MetricValue", "Please enter a value for quantitative observations.");
                    await LoadDropdownsAsync();
                    return Page();
                }

                if (MetricValue < observationDefinition.MinimumValue || MetricValue > observationDefinition.MaximumValue)
                {
                    ModelState.AddModelError("MetricValue", $"Value must be between {observationDefinition.MinimumValue} and {observationDefinition.MaximumValue}.");
                    await LoadDropdownsAsync();
                    return Page();
                }
            }

            var dogRecord = new DogRecord
            {
                DogId = DogId,
                MetricTypeId = MetricTypeId,
                MetricValue = MetricValue,
                Note = Note,
                RecordTime = RecordTime ?? DateTime.Now,
                CreatedBy = User.Identity?.Name ?? "Unknown"
            };

            _context.DogRecords.Add(dogRecord);
            await _context.SaveChangesAsync();

            if (SelectedMetaTagIds.Any())
            {
                var metaTags = await _context.MetaTags
                    .Where(mt => SelectedMetaTagIds.Contains(mt.Id))
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
            ObservationDefinitions = new SelectList(
                await _context.ObservationDefinitions
                    .Where(od => od.IsActive == true)
                    .Select(od => new SelectListItem { Value = od.Id.ToString(), Text = od.DefinitionName })
                    .ToListAsync(),
                "Value",
                "Text",
                ObservationDefinitionId?.ToString());

            if (ObservationDefinitionId.HasValue)
            {
                var observationDefinition = await _context.ObservationDefinitions
                    .FirstOrDefaultAsync(od => od.Id == ObservationDefinitionId);
                IsQualitative = observationDefinition?.IsQualitative ?? false;

                MetricTypes = new SelectList(
                    await _context.MetricTypes
                        .Where(mt => mt.ObservationDefinitionId == ObservationDefinitionId && mt.IsActive == true)
                        .Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.Description })
                        .ToListAsync(),
                    "Value",
                    "Text",
                    MetricTypeId?.ToString());
            }
            else
            {
                MetricTypes = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            MetaTags = new SelectList(
                await _context.MetaTags
                    .Where(mt => mt.IsActive == true)
                    .Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.TagName })
                    .ToListAsync(),
                "Value",
                "Text");
        }
    }
}