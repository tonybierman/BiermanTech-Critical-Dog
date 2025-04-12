using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.ObservationTypes
{
    public class EditModel : PageModel
    {
        private readonly IObservationTypeService _observationTypeService;

        public EditModel(IObservationTypeService observationTypeService)
        {
            _observationTypeService = observationTypeService;
        }

        [BindProperty]
        public ObservationTypeInputViewModel ObservationTypeVM { get; set; } = new ObservationTypeInputViewModel();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ObservationTypeVM = await _observationTypeService.GetObservationTypeViewModelByIdAsync(id);
            if (ObservationTypeVM == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _observationTypeService.UpdateObservationTypeAsync(ObservationTypeVM);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}