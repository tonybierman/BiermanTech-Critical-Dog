using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.ObservationTypes
{
    public class DeleteModel : PageModel
    {
        private readonly IObservationTypeService _observationTypeService;

        public DeleteModel(IObservationTypeService observationTypeService)
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
            try
            {
                await _observationTypeService.DeleteObservationTypeAsync(ObservationTypeVM.Id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}