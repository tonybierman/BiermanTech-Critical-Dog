using Microsoft.EntityFrameworkCore;
using BiermanTech.CriticalDog.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Analytics
{
    public class WeightAnalyticsProvider : IWeightAnalyticsProvider
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WeightAnalyticsProvider> _logger;

        public WeightAnalyticsProvider(AppDbContext context, ILogger<WeightAnalyticsProvider> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<WeightChangeReport> GetWeightChangeReportAsync(int subjectId)
        {
            _logger.LogInformation($"Generating weight change report for SubjectId: {subjectId}");

            try
            {
                // Fetch the subject
                var subject = await _context.GetFilteredSubjects()
                    .Where(s => s.Id == subjectId)
                    .FirstOrDefaultAsync();

                if (subject == null)
                {
                    _logger.LogWarning($"Subject {subjectId} not found.");
                    throw new KeyNotFoundException("Subject not found or not authorized.");
                }

                // Fetch WeighIn observation definition
                var weighInDef = await _context.ObservationDefinitions
                    .Where(od => od.DefinitionName == "WeighIn" && od.IsActive == true)
                    .FirstOrDefaultAsync();

                if (weighInDef == null)
                {
                    _logger.LogError("WeighIn ObservationDefinition not found.");
                    throw new InvalidOperationException("WeighIn ObservationDefinition not found.");
                }

                // Fetch all WeighIn records for the subject
                var records = await _context.GetFilteredSubjectRecords()
                    .Where(sr => sr.SubjectId == subjectId && sr.ObservationDefinitionId == weighInDef.Id && sr.MetricTypeId != null)
                    .Include(sr => sr.MetricType)
                    .ThenInclude(mt => mt.Unit)
                    .OrderBy(sr => sr.RecordTime)
                    .ToListAsync();

                if (!records.Any())
                {
                    _logger.LogInformation($"No WeighIn records found for Subject: {subject.Name}");
                    return new WeightChangeReport
                    {
                        SubjectName = subject.Name,
                        TrendDescription = "No weight observations available."
                    };
                }

                // Convert all weights to Kilograms
                var observations = records.Select(r => new WeightObservation
                {
                    RecordTime = r.RecordTime,
                    WeightKg = ConvertToKilograms(r.MetricValue ?? 0, r.MetricType?.Unit?.UnitName)
                }).OrderBy(o => o.RecordTime).ToList();

                // Calculate weight change rates
                double? averageRateKgPerDay = CalculateAverageRate(observations);

                // Determine trend
                string trendDescription = DetermineTrend(observations, averageRateKgPerDay);

                return new WeightChangeReport
                {
                    SubjectName = subject.Name,
                    Observations = observations,
                    AverageRateKgPerDay = averageRateKgPerDay,
                    TrendDescription = trendDescription
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating weight change report for SubjectId: {subjectId}");
                throw;
            }
        }

        private decimal ConvertToKilograms(decimal value, string? unitName)
        {
            if (string.IsNullOrEmpty(unitName))
                return value; // Assume Kilograms if unit is missing

            return unitName switch
            {
                "Kilograms" => value,
                "Grams" => value / 1000m,
                "Milligrams" => value / 1_000_000m,
                "Pounds" => value * 0.45359237m,
                "Ounces" => value * 0.0283495231m,
                _ => value // Default to Kilograms if unknown
            };
        }

        private double? CalculateAverageRate(List<WeightObservation> observations)
        {
            if (observations.Count < 2)
                return null;

            double totalRate = 0;
            int intervals = 0;

            for (int i = 1; i < observations.Count; i++)
            {
                var prev = observations[i - 1];
                var curr = observations[i];
                var timeSpan = curr.RecordTime - prev.RecordTime;
                var days = timeSpan.TotalDays;

                if (days > 0) // Avoid division by zero
                {
                    var weightChange = curr.WeightKg - prev.WeightKg;
                    var rate = (double)(weightChange / (decimal)days);
                    totalRate += rate;
                    intervals++;
                }
            }

            return intervals > 0 ? totalRate / intervals : null;
        }

        private string DetermineTrend(List<WeightObservation> observations, double? averageRate)
        {
            if (observations.Count < 2)
                return "Insufficient data to determine trend.";

            if (averageRate == null)
                return "No valid intervals to determine trend.";

            if (averageRate > 0.01)
                return $"Gaining weight at an average rate of {averageRate:F3} kg/day.";
            else if (averageRate < -0.01)
                return $"Losing weight at an average rate of {Math.Abs(averageRate.Value):F3} kg/day.";
            else
                return "Weight is stable.";
        }
    }
}