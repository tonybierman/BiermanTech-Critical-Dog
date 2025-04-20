using Microsoft.EntityFrameworkCore;
using BiermanTech.CriticalDog.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Analytics
{
    public class ObservationAnalyticsProvider : IObservationAnalyticsProvider
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ObservationAnalyticsProvider> _logger;

        public ObservationAnalyticsProvider(AppDbContext context, ILogger<ObservationAnalyticsProvider> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ObservationChangeReport> GetObservationChangeReportAsync(int subjectId, string observationDefinitionName)
        {
            _logger.LogInformation($"Generating observation change report for SubjectId: {subjectId}, Observation: {observationDefinitionName}");

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

                // Fetch observation definition
                var observationDef = await _context.ObservationDefinitions
                    .Where(od => od.DefinitionName == observationDefinitionName && od.IsActive == true)
                    .FirstOrDefaultAsync();

                if (observationDef == null)
                {
                    _logger.LogError($"ObservationDefinition '{observationDefinitionName}' not found.");
                    throw new InvalidOperationException($"ObservationDefinition '{observationDefinitionName}' not found.");
                }

                // Fetch records for the observation
                var records = await _context.GetFilteredSubjectRecords()
                    .Where(sr => sr.SubjectId == subjectId && sr.ObservationDefinitionId == observationDef.Id && sr.MetricTypeId != null)
                    .Include(sr => sr.MetricType)
                    .ThenInclude(mt => mt.Unit)
                    .OrderBy(sr => sr.RecordTime)
                    .ToListAsync();

                if (!records.Any())
                {
                    _logger.LogInformation($"No {observationDefinitionName} records found for Subject: {subject.Name}");
                    return new ObservationChangeReport
                    {
                        SubjectName = subject.Name,
                        ObservationType = observationDefinitionName,
                        UnitName = "N/A",
                        TrendDescription = $"No {observationDefinitionName} observations available."
                    };
                }

                // Determine the primary unit (use the most common unit or first available)
                var primaryUnit = records
                    .GroupBy(r => r.MetricType?.Unit?.UnitName)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault() ?? "Unknown";

                // Convert values to the primary unit and calculate percent change per week
                var observations = new List<Observation>();
                for (int i = 0; i < records.Count; i++)
                {
                    double? percentChangePerWeek = null;
                    if (i > 0)
                    {
                        var prev = records[i - 1];
                        var curr = records[i];
                        var timeSpan = curr.RecordTime - prev.RecordTime;
                        var days = timeSpan.TotalDays;

                        if (days > 0 && prev.MetricValue != 0)
                        {
                            var prevValue = ConvertToPrimaryUnit(prev.MetricValue ?? 0, prev.MetricType?.Unit?.UnitName, primaryUnit, observationDefinitionName);
                            var currValue = ConvertToPrimaryUnit(curr.MetricValue ?? 0, curr.MetricType?.Unit?.UnitName, primaryUnit, observationDefinitionName);
                            var percentChange = ((currValue - prevValue) / prevValue) * 100;
                            percentChangePerWeek = (double)percentChange * (7.0 / days); // Normalize to weekly rate
                        }
                    }

                    observations.Add(new Observation
                    {
                        RecordTime = records[i].RecordTime,
                        Value = ConvertToPrimaryUnit(records[i].MetricValue ?? 0, records[i].MetricType?.Unit?.UnitName, primaryUnit, observationDefinitionName),
                        PercentChangePerWeek = percentChangePerWeek
                    });
                }

                // Calculate average rate (in units/day)
                double? averageRatePerDay = CalculateAverageRate(observations);

                // Determine trend
                string trendDescription = DetermineTrend(observations, averageRatePerDay);

                return new ObservationChangeReport
                {
                    SubjectName = subject.Name,
                    ObservationType = observationDefinitionName,
                    UnitName = primaryUnit,
                    Observations = observations,
                    AverageRatePerDay = averageRatePerDay,
                    TrendDescription = trendDescription
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating observation change report for SubjectId: {subjectId}, Observation: {observationDefinitionName}");
                throw;
            }
        }

        private decimal ConvertToPrimaryUnit(decimal value, string? sourceUnit, string targetUnit, string observationDefinitionName)
        {
            if (string.IsNullOrEmpty(sourceUnit) || sourceUnit == targetUnit)
                return value;

            // Conversion logic based on observation type
            return observationDefinitionName switch
            {
                "WeighIn" => ConvertWeight(value, sourceUnit, targetUnit),
                "TempCheck" => ConvertTemperature(value, sourceUnit, targetUnit),
                "HeartRate" or "RespiratoryRate" => sourceUnit == targetUnit ? value : throw new InvalidOperationException($"No conversion available for {sourceUnit} to {targetUnit}"),
                // Add other observation types as needed
                _ => value // Default: no conversion if unknown
            };
        }

        private decimal ConvertWeight(decimal value, string sourceUnit, string targetUnit)
        {
            // Convert to Kilograms first
            decimal inKilograms = sourceUnit switch
            {
                "Kilograms" => value,
                "Grams" => value / 1000m,
                "Milligrams" => value / 1_000_000m,
                "Pounds" => value * 0.45359237m,
                "Ounces" => value * 0.0283495231m,
                _ => value
            };

            // Convert from Kilograms to target unit
            return targetUnit switch
            {
                "Kilograms" => inKilograms,
                "Grams" => inKilograms * 1000m,
                "Milligrams" => inKilograms * 1_000_000m,
                "Pounds" => inKilograms / 0.45359237m,
                "Ounces" => inKilograms / 0.0283495231m,
                _ => inKilograms
            };
        }

        private decimal ConvertTemperature(decimal value, string sourceUnit, string targetUnit)
        {
            // Convert to Celsius first
            decimal inCelsius = sourceUnit switch
            {
                "DegreesCelsius" => value,
                "DegreesFahrenheit" => (value - 32m) * 5m / 9m,
                _ => value
            };

            // Convert from Celsius to target unit
            return targetUnit switch
            {
                "DegreesCelsius" => inCelsius,
                "DegreesFahrenheit" => (inCelsius * 9m / 5m) + 32m,
                _ => inCelsius
            };
        }

        private double? CalculateAverageRate(List<Observation> observations)
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

                if (days > 0)
                {
                    var valueChange = curr.Value - prev.Value;
                    var rate = (double)(valueChange / (decimal)days);
                    totalRate += rate;
                    intervals++;
                }
            }

            return intervals > 0 ? totalRate / intervals : null;
        }

        private string DetermineTrend(List<Observation> observations, double? averageRate)
        {
            if (observations.Count < 2)
                return "Insufficient data to determine trend.";

            if (averageRate == null)
                return "No valid intervals to determine trend.";

            if (averageRate > 0.01)
                return $"Increasing at an average rate of {averageRate:F3} units/day.";
            else if (averageRate < -0.01)
                return $"Decreasing at an average rate of {Math.Abs(averageRate.Value):F3} units/day.";
            else
                return "Stable.";
        }
    }
}