using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.Units
{
    public class DetailsModel : PageModel
    {
        private readonly IUnitService _unitService;

        public DetailsModel(IUnitService unitService)
        {
            _unitService = unitService;
        }

        public UnitInputViewModel UnitVM { get; set; } = new UnitInputViewModel();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            UnitVM = await _unitService.GetUnitViewModelByIdAsync(id);
            if (UnitVM == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}