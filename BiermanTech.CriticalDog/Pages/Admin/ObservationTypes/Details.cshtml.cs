using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.ObservationTypes
{
    public class DetailsModel : PageModel
    {
        private readonly IObservationTypeService _observationTypeService;

        public DetailsModel(IObservationTypeService observationTypeService)
        {
            _observationTypeService = observationTypeService;
        }

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
    }
}