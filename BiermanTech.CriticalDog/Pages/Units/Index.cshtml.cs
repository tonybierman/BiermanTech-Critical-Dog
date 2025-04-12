using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Units
{
    public class IndexModel : PageModel
    {
        private readonly IUnitService _unitService;

        public IndexModel(IUnitService unitService)
        {
            _unitService = unitService;
        }

        public List<UnitInputViewModel> Units { get; set; } = new List<UnitInputViewModel>();

        public async Task OnGetAsync()
        {
            Units = await _unitService.GetAllUnitsAsync();
        }
    }
}