using AutoMapper;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.ObservationDefinitions
{
    public class DeleteModel : PageModel
    {
        private readonly IObservationDefinitionService _definitionService;
        private readonly IMapper _mapper;

        public DeleteModel(IObservationDefinitionService definitionService, IMapper mapper)
        {
            _definitionService = definitionService;
            _mapper = mapper;
        }

        [BindProperty]
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
            ObservationTypeName = definition.ObservationType?.Name ?? "Unknown";
            ScientificDisciplineNames = definition.ScientificDisciplines
                .Select(d => d.Name)
                .ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _definitionService.DeleteDefinitionAsync(DefinitionVM.Id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}