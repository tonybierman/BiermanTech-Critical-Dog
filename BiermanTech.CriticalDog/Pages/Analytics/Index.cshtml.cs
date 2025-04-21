using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BiermanTech.CriticalDog.Analytics;
using System;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Analytics
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IObservationAnalyticsProvider _analyticsProvider;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IObservationAnalyticsProvider analyticsProvider, ILogger<IndexModel> logger)
        {
            _analyticsProvider = analyticsProvider;
            _logger = logger;
        }

        public ObservationChangeReport Report { get; set; } = new ObservationChangeReport();

        [BindProperty(SupportsGet = true)]
        public int SubjectId { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string ObservationDefinitionName { get; set; } = string.Empty;
        
        [BindProperty(SupportsGet = true)]
        public string? Unit { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(ObservationDefinitionName))
            {
                ModelState.AddModelError(string.Empty, "Observation type is required.");
                return Page();
            }

            try
            {
                Report = await _analyticsProvider.GetObservationChangeReportAsync(SubjectId, ObservationDefinitionName, Unit);
                return Page();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Subject {SubjectId} not found or not authorized.");
                return NotFound("Subject not found or not authorized.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Invalid observation type or unit: {ObservationDefinitionName}, {Unit}");
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating observation report for SubjectId: {SubjectId}, Observation: {ObservationDefinitionName}");
                ModelState.AddModelError(string.Empty, "An error occurred while generating the report.");
                return Page();
            }
        }
    }
}