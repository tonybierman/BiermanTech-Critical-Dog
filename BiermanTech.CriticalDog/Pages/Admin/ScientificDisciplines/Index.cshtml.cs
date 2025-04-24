using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.ScientificDisciplines
{
    public class IndexModel : PageModel
    {
        private readonly IScientificDisciplineService _disciplineService;

        public IndexModel(IScientificDisciplineService disciplineService)
        {
            _disciplineService = disciplineService;
        }

        public List<ScientificDisciplineInputViewModel> Disciplines { get; set; } = new List<ScientificDisciplineInputViewModel>();

        public async Task OnGetAsync()
        {
            Disciplines = await _disciplineService.GetAllDisciplinesAsync();
        }
    }
}