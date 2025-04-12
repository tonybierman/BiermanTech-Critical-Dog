using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.Units
{
    public class CreateModel : PageModel
    {
        private readonly IUnitService _unitService;

        public CreateModel(IUnitService unitService)
        {
            _unitService = unitService;
        }

        [BindProperty]
        public UnitInputViewModel UnitVM { get; set; } = new UnitInputViewModel();

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

            await _unitService.CreateUnitAsync(UnitVM);
            return RedirectToPage("./Index");
        }
    }
}