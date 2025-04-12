using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.ObservationTypes
{
    public class CreateModel : PageModel
    {
        private readonly IObservationTypeService _observationTypeService;

        public CreateModel(IObservationTypeService observationTypeService)
        {
            _observationTypeService = observationTypeService;
        }

        [BindProperty]
        public ObservationTypeInputViewModel ObservationTypeVM { get; set; } = new ObservationTypeInputViewModel();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _observationTypeService.CreateObservationTypeAsync(ObservationTypeVM);
            return RedirectToPage("./Index");
        }
    }
}