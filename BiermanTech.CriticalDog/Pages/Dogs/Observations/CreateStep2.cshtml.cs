using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations
{
    public class CreateStep2Model : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CreateStep2Model> _logger;

        public CreateStep2Model(AppDbContext context, ILogger<CreateStep2Model> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public CreateObservationViewModel Observation { get; set; } = new CreateObservationViewModel();

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
            Observation.IsQualitative = observationDefinition.IsQualitative;
            Observation.MinValue = observationDefinition.MinimumValue;
            Observation.MaxValue = observationDefinition.MaximumValue;

            Observation.RecordTime = DateTime.Now;

            await LoadDropdownsAsync();
            TempData.Keep("Observation"); // Keep TempData for the next step
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
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

                if (Observation.MetricValue < observationDefinition.MinimumValue || Observation.MetricValue > observationDefinition.MaximumValue)
                {
                    ModelState.AddModelError("Observation.MetricValue", $"Value must be between {observationDefinition.MinimumValue} and {observationDefinition.MaximumValue}.");
                    await LoadDropdownsAsync();
                    return Page();
                }
            }

            // Store the updated observation in TempData
            TempData["Observation"] = System.Text.Json.JsonSerializer.Serialize(Observation);
            return RedirectToPage("CreateStep3", new { dogId });
        }

        private async Task LoadDropdownsAsync()
        {
            Observation.MetricTypes = new SelectList(
                await _context.MetricTypes
                    .Where(mt => mt.ObservationDefinitionId == Observation.ObservationDefinitionId && mt.IsActive == true)
                    .Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.Description })
                    .ToListAsync(),
                "Value",
                "Text",
                Observation.MetricTypeId?.ToString());
        }
    }
}