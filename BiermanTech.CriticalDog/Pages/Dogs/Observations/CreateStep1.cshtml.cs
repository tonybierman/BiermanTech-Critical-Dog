using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations
{
    public class CreateStep1Model : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CreateStep1Model> _logger;

        public CreateStep1Model(AppDbContext context, ILogger<CreateStep1Model> logger)
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

            await LoadDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int dogId)
        {
            if (!Observation.ObservationDefinitionId.HasValue)
            {
                ModelState.AddModelError("Observation.ObservationDefinitionId", "Please select an observation type.");
                await LoadDropdownsAsync();
                return Page();
            }

            // Store the observation in TempData or session to pass to the next step
            TempData["Observation"] = System.Text.Json.JsonSerializer.Serialize(Observation);
            return RedirectToPage("CreateStep2", new { dogId });
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
        }
    }
}