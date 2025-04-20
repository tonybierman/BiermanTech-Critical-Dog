using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BiermanTech.CriticalDog.Analytics;
using System;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Subjects
{
    [Authorize]
    public class WeightReportModel : PageModel
    {
        private readonly IWeightAnalyticsProvider _analyticsProvider;
        private readonly ILogger<WeightReportModel> _logger;

        public WeightReportModel(IWeightAnalyticsProvider analyticsProvider, ILogger<WeightReportModel> logger)
        {
            _analyticsProvider = analyticsProvider;
            _logger = logger;
        }

        public WeightChangeReport Report { get; set; } = new WeightChangeReport();
        [BindProperty(SupportsGet = true)]
        public int SubjectId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Fetch the weight change report
                Report = await _analyticsProvider.GetWeightChangeReportAsync(SubjectId);
                return Page();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Subject {SubjectId} not found or not authorized.");
                return NotFound("Subject not found or not authorized.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating weight report for SubjectId: {SubjectId}");
                ModelState.AddModelError(string.Empty, "An error occurred while generating the report.");
                return Page();
            }
        }
    }
}