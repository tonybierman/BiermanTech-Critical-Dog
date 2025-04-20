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
        private readonly IUnitConverter _unitConverter;

        public ObservationAnalyticsProvider(AppDbContext context, ILogger<ObservationAnalyticsProvider> logger, IUnitConverter unitConverter)
        {
            _context = context;
            _logger = logger;
            _unitConverter = unitConverter;
        }

        public async Task<ObservationChangeReport> GetObservationChangeReportAsync(int subjectId, string observationDefinitionName, string? displayUnitName = null)
        {
            _logger.LogInformation($"Generating observation change report for SubjectId: {subjectId}, Observation: {observationDefinitionName}, DisplayUnit: {displayUnitName ?? "default"}");

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

                // Fetch observation definition with units
                var observationDef = await _context.ObservationDefinitions
                    .Where(od => od.DefinitionName == observationDefinitionName && od.IsActive == true)
                    .Include(od => od.Units)
                    .FirstOrDefaultAsync();

                if (observationDef == null)
                {
                    _logger.LogError($"ObservationDefinition '{observationDefinitionName}' not found.");
                    throw new InvalidOperationException($"ObservationDefinition '{observationDefinitionName}' not found.");
                }

                // Get supported units
                var supportedUnits = observationDef.Units.Select(u => u.UnitName).ToList();
                if (!supportedUnits.Any())
                {
                    _logger.LogWarning($"No units defined for ObservationDefinition '{observationDefinitionName}'.");
                    throw new InvalidOperationException($"No units defined for '{observationDefinitionName}'.");
                }

                // Determine standard unit
                string standardUnitName = GetStandardUnit(observationDefinitionName, supportedUnits);

                // Validate display unit
                string selectedDisplayUnit = displayUnitName != null && supportedUnits.Contains(displayUnitName)
                    ? displayUnitName
                    : standardUnitName;

                // Fetch records
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
                        StandardUnitName = standardUnitName,
                        DisplayUnitName = selectedDisplayUnit,
                        TrendDescription = $"No {observationDefinitionName} observations available."
                    };
                }

                // Process records
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
                            var prevValue = await ConvertValueAsync(prev.MetricValue ?? 0, prev.MetricType?.Unit?.UnitName, standardUnitName, observationDefinitionName);
                            var currValue = await ConvertValueAsync(curr.MetricValue ?? 0, curr.MetricType?.Unit?.UnitName, standardUnitName, observationDefinitionName);
                            var percentChange = ((currValue - prevValue) / prevValue) * 100;
                            percentChangePerWeek = (double)(percentChange * (7m / (decimal)days));
                        }
                    }

                    observations.Add(new Observation
                    {
                        RecordTime = records[i].RecordTime,
                        Value = await ConvertValueAsync(records[i].MetricValue ?? 0, records[i].MetricType?.Unit?.UnitName, selectedDisplayUnit, observationDefinitionName),
                        PercentChangePerWeek = percentChangePerWeek
                    });
                }

                // Calculate average rate
                double? averageRatePerDay = await CalculateAverageRateAsync(observations, standardUnitName, selectedDisplayUnit, observationDefinitionName);

                // Determine trend
                string trendDescription = DetermineTrend(observations, averageRatePerDay);

                return new ObservationChangeReport
                {
                    SubjectName = subject.Name,
                    ObservationType = observationDefinitionName,
                    StandardUnitName = standardUnitName,
                    DisplayUnitName = selectedDisplayUnit,
                    Observations = observations,
                    AverageRatePerDay = averageRatePerDay,
                    TrendDescription = trendDescription
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating observation report for SubjectId: {subjectId}, Observation: {observationDefinitionName}");
                throw;
            }
        }

        private string GetStandardUnit(string observationDefinitionName, List<string> supportedUnits)
        {
            return observationDefinitionName switch
            {
                "WeighIn" => supportedUnits.Contains("Kilograms") ? "Kilograms" : supportedUnits.First(),
                "TempCheck" => supportedUnits.Contains("DegreesCelsius") ? "DegreesCelsius" : supportedUnits.First(),
                "HeartRate" or "RespiratoryRate" => supportedUnits.Contains("BeatsPerMinute") ? "BeatsPerMinute" : supportedUnits.First(),
                _ => supportedUnits.First()
            };
        }

        private async Task<decimal> ConvertValueAsync(decimal value, string? sourceUnit, string targetUnit, string observationDefinitionName)
        {
            if (string.IsNullOrEmpty(sourceUnit) || sourceUnit == targetUnit)
                return value;

            try
            {
                double convertedValue = await _unitConverter.ConvertAsync(sourceUnit, targetUnit, (double)value);
                return (decimal)convertedValue;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"No conversion rule for {sourceUnit} to {targetUnit} for {observationDefinitionName}. Returning original value. Error: {ex.Message}");
                return value;
            }
        }

        private async Task<double?> CalculateAverageRateAsync(List<Observation> observations, string standardUnitName, string displayUnitName, string observationDefinitionName)
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
                    var prevValue = await ConvertValueAsync(prev.Value, displayUnitName, standardUnitName, observationDefinitionName);
                    var currValue = await ConvertValueAsync(curr.Value, displayUnitName, standardUnitName, observationDefinitionName);
                    var valueChange = currValue - prevValue;
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