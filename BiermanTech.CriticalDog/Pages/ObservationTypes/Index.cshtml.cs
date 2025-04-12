using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.ObservationTypes
{
    public class IndexModel : PageModel
    {
        private readonly IObservationTypeService _observationTypeService;

        public IndexModel(IObservationTypeService observationTypeService)
        {
            _observationTypeService = observationTypeService;
        }

        public List<ObservationTypeInputViewModel> ObservationTypes { get; set; } = new List<ObservationTypeInputViewModel>();

        public async Task OnGetAsync()
        {
            ObservationTypes = await _observationTypeService.GetAllObservationTypesAsync();
        }
    }
}