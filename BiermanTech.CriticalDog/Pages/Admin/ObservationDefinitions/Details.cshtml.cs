using AutoMapper;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BiermanTech.CriticalDog.Pages.Admin.ObservationDefinitions
{
    public class DetailsModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IObservationDefinitionService _definitionService;
        private readonly IMapper _mapper;
        public MarkdownRendererPartialViewModel MarkdownHelpVM { get; set; }

        public DetailsModel(IObservationDefinitionService definitionService, IWebHostEnvironment environment, IMapper mapper)
        {
            _environment = environment;
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
            ObservationTypeName = definition.ObservationType?.Name ?? "Unknown";
            ScientificDisciplineNames = definition.ScientificDisciplines
                .Select(d => d.Name)
                .ToList();

            // Construct markdown filename and initialize MarkdownHelpVM
            string markdownFileName = null;
            if (!string.IsNullOrEmpty(DefinitionVM?.Name))
            {
                markdownFileName = $"{DefinitionVM.Name.Replace(" ", "_")}.md";
            }

            MarkdownHelpVM = new MarkdownRendererPartialViewModel(
                markdownFileName,
                _environment.WebRootPath
            );

            return Page();
        }
    }
}