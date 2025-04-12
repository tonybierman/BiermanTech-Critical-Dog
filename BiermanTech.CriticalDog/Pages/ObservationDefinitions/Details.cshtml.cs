using AutoMapper;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.ObservationDefinitions
{
    public class DetailsModel : PageModel
    {
        private readonly IObservationDefinitionService _definitionService;
        private readonly IMapper _mapper;

        public DetailsModel(IObservationDefinitionService definitionService, IMapper mapper)
        {
            _definitionService = definitionService;
            _mapper = mapper;
        }

        public ObservationDefinitionInputViewModel DefinitionVM { get; set; } = new ObservationDefinitionInputViewModel();
        public string ObservationTypeName { get; set; }
        public List<string> ScientificDisciplineNames { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var definition = await _definitionService.GetDefinitionByIdAsync(id);
            if (definition == null)
            {
                return NotFound();
            }

            DefinitionVM = _mapper.Map<ObservationDefinitionInputViewModel>(definition);
            ObservationTypeName = definition.ObservationType?.TypeName ?? "Unknown";
            ScientificDisciplineNames = definition.ScientificDisciplines
                .Select(d => d.DisciplineName)
                .ToList();

            return Page();
        }
    }
}