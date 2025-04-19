using AutoMapper;
using BiermanTech.CriticalDog.Services;
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
        public MarkdownHelpViewModel MarkdownHelpVM { get; set; }

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
            ObservationTypeName = definition.ObservationType?.TypeName ?? "Unknown";
            ScientificDisciplineNames = definition.ScientificDisciplines
                .Select(d => d.DisciplineName)
                .ToList();

            // Initialize MarkdownHelpVM
            MarkdownHelpVM = new MarkdownHelpViewModel();

            // Load and convert markdown file based on DefinitionName
            if (!string.IsNullOrEmpty(DefinitionVM?.DefinitionName))
            {
                var markdownFileName = $"{DefinitionVM.DefinitionName.Replace(" ", "_")}.md";
                var markdownFilePath = Path.Combine(_environment.WebRootPath, "help", markdownFileName);

                if (System.IO.File.Exists(markdownFilePath))
                {
                    var markdownContent = await System.IO.File.ReadAllTextAsync(markdownFilePath);
                    var pipeline = new MarkdownPipelineBuilder()
                        .UseAdvancedExtensions()
                        .Build();
                    MarkdownHelpVM.MarkdownHtml = Markdown.ToHtml(markdownContent, pipeline);
                }
            }

            return Page();
        }
    }
}