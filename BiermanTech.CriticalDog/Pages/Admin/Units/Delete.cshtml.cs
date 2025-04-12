using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.Units
{
    public class DeleteModel : PageModel
    {
        private readonly IUnitService _unitService;

        public DeleteModel(IUnitService unitService)
        {
            _unitService = unitService;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _unitService.DeleteUnitAsync(UnitVM.Id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}