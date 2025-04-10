using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations
{
    public class CreateStep3Model : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CreateStep3Model> _logger;

        public CreateStep3Model(AppDbContext context, ILogger<CreateStep3Model> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public CreateObservationViewModel Observation { get; set; } = new CreateObservationViewModel();

        public string ObservationDefinitionName { get; set; }
        public string MetricTypeDescription { get; set; }

        public async Task<IActionResult> OnGetAsync(int dogId)
        {
            if (TempData["Observation"] == null)
            {
                return RedirectToPage("CreateStep1", new { dogId });
            }

            Observation = System.Text.Json.JsonSerializer.Deserialize<CreateObservationViewModel>(TempData["Observation"].ToString());
            Observation.DogId = dogId;

            var dog = await _context.Dogs.FindAsync(dogId);
            if (dog == null)
            {
                return NotFound();
            }
            Observation.DogName = dog.Name ?? "Unknown";

            var observationDefinition = await _context.ObservationDefinitions
                .FirstOrDefaultAsync(od => od.Id == Observation.ObservationDefinitionId);
            if (observationDefinition == null)
            {
                return NotFound();
            }
            ObservationDefinitionName = observationDefinition.DefinitionName;

            if (!Observation.IsQualitative && Observation.MetricTypeId.HasValue)
            {
                var metricType = await _context.MetricTypes
                    .FirstOrDefaultAsync(mt => mt.Id == Observation.MetricTypeId);
                MetricTypeDescription = metricType?.Description ?? "Unknown";
            }

            await LoadDropdownsAsync();
            TempData.Keep("Observation");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
            //if (ModelState.IsValid)
            if (true)
            {
                _logger.LogInformation("Saving observation for DogId: {DogId}, ObservationDefinitionId: {ObservationDefinitionId}, SelectedMetaTagIds: {SelectedMetaTagIds}",
                    Observation.DogId, Observation.ObservationDefinitionId, string.Join(", ", Observation.SelectedMetaTagIds ?? new List<int>()));

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
                    foreach (var tagId in Observation.SelectedMetaTagIds)
                    {
                        var metaTagExists = await _context.MetaTags.FirstOrDefaultAsync(mt => mt.Id == tagId);
                        if (metaTagExists != null)
                        {
                            dogRecord.MetaTags.Add(metaTagExists);
                        }
                        else
                        {
                            _logger.LogWarning("MetaTag with Id {TagId} not found.", tagId);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                TempData.Remove("Observation");
                return RedirectToPage("/Dogs/Index");
            }

            await LoadDropdownsAsync();
            return Page();
        }

        private async Task LoadDropdownsAsync()
        {
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