using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BiermanTech.CriticalDog.Data;

namespace BiermanTech.CriticalDog.Data
{
    public static class UserDbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, string userId, bool subjectsOnly, ILogger? logger = null)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            logger ??= serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation($"Initializing user data for UserId: {userId}...");

                await SeedUserDataAsync(context, userId, logger, subjectsOnly);

                logger.LogInformation("User data initialization completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while initializing user data for UserId: {userId}.");
                throw;
            }
            finally
            {
                // Ensure context is disposed to release connections
                await context.DisposeAsync();
                logger.LogInformation("Disposed AppDbContext after UserDbInitializer.");
            }
        }

        private static async Task SeedUserDataAsync(AppDbContext context, string userId, ILogger logger, bool subjectsOnly)
        {
            // Get SubjectType for "Dog"
            var dogSubjectType = await context.SubjectTypes.FirstOrDefaultAsync(st => st.TypeName == "Dog");
            if (dogSubjectType == null)
            {
                logger.LogError("SubjectType 'Dog' not found. Ensure AppDbInitializer has been run.");
                throw new InvalidOperationException("SubjectType 'Dog' not found.");
            }

            // Check for existing subjects
            var subjectsQuery = await context.GetFilteredSubjects(userId);
            var subjects = await subjectsQuery.ToListAsync();

            // Seed Subjects (Dogs)
            if (!subjects.Any())
            {
                logger.LogInformation("Seeding Subjects...");
                var newSubjects = new List<Subject>
                {
                    new Subject
                    {
                        Name = "Max",
                        Breed = "Labrador Retriever",
                        Sex = 1, // Male
                        ArrivalDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-3)),
                        Notes = "Friendly and energetic",
                        SubjectTypeId = dogSubjectType.Id,
                        UserId = userId,
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Subject
                    {
                        Name = "Bella",
                        Breed = "German Shepherd",
                        Sex = 2, // Female
                        ArrivalDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-2)),
                        Notes = "Loyal and protective",
                        SubjectTypeId = dogSubjectType.Id,
                        UserId = userId,
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.AddRange(newSubjects);
                await RetryAsync(() => context.SaveChangesAsync(), logger, "Saving subjects");
                logger.LogInformation("Subjects seeded successfully.");
            }

            if (subjectsOnly)
                return;

            // Refresh subjects list after seeding
            subjects = await (await context.GetFilteredSubjects(userId)).ToListAsync();
            if (!subjects.Any())
            {
                logger.LogError("No subjects found for the user. Cannot seed SubjectRecords.");
                return;
            }

            var observationDefinitions = await context.ObservationDefinitions.Where(od => od.IsActive == true).ToListAsync();
            var metricTypes = await context.MetricTypes.Where(mt => mt.IsActive == true).ToListAsync();
            var metaTags = await context.MetaTags.Where(mt => mt.IsActive == true).ToListAsync();
            var random = new Random();

            foreach (var subject in subjects)
            {
                logger.LogInformation($"Seeding 50 SubjectRecords for Subject: {subject.Name}...");
                var records = new List<SubjectRecord>();
                const int batchSize = 5;

                for (int i = 0; i < 50; i++)
                {
                    // Randomly select an ObservationDefinition
                    var obsDef = observationDefinitions[random.Next(observationDefinitions.Count())];
                    var validMetricTypes = metricTypes.Where(mt => mt.ObservationDefinitionId == obsDef.Id).ToList();
                    MetricType? metricType = validMetricTypes.Any() ? validMetricTypes[random.Next(validMetricTypes.Count())] : null;

                    // Generate MetricValue within the defined range
                    decimal? metricValue = null;
                    var minValue = obsDef.MinimumValue ?? 0m;
                    var maxValue = obsDef.MaximumValue ?? 100m;
                    metricValue = minValue + (decimal)random.NextDouble() * (maxValue - minValue);
                    metricValue = Math.Round(metricValue.Value, 2);

                    // Generate random RecordTime within the last 30 days
                    var daysAgo = random.Next(0, 30);
                    var hours = random.Next(0, 24);
                    var minutes = random.Next(0, 60);
                    var recordTime = DateTime.UtcNow.AddDays(-daysAgo).AddHours(hours).AddMinutes(minutes);

                    // Generate random Note
                    string? note = null;
                    if (random.NextDouble() > 0.5)
                    {
                        note = $"Observation for {obsDef.DefinitionName}: {GenerateRandomNote(obsDef.DefinitionName)}";
                    }

                    // Assign random MetaTags (0 to 3 tags)
                    var selectedMetaTags = metaTags.OrderBy(_ => random.Next()).Take(random.Next(0, 4)).ToList();

                    var record = new SubjectRecord
                    {
                        SubjectId = subject.Id,
                        ObservationDefinitionId = obsDef.Id,
                        MetricTypeId = metricType?.Id,
                        MetricValue = metricValue,
                        Note = note,
                        RecordTime = recordTime,
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow,
                        MetaTags = selectedMetaTags
                    };

                    records.Add(record);

                    // Save in batches of 5
                    if (records.Count >= batchSize || i == 49)
                    {
                        context.AddRange(records);
                        await RetryAsync(() => context.SaveChangesAsync(), logger, $"Saving batch of {records.Count} records for {subject.Name}");
                        records.Clear();
                        logger.LogInformation($"Saved batch of {batchSize} records for Subject: {subject.Name}.");
                    }
                }
            }
        }

        private static async Task RetryAsync(Func<Task> operation, ILogger logger, string operationName, int maxAttempts = 3, int delayMs = 500)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    await operation();
                    return;
                }
                catch (Exception ex) when (attempt < maxAttempts)
                {
                    logger.LogWarning(ex, $"Attempt {attempt} failed for {operationName}. Retrying in {delayMs}ms...");
                    await Task.Delay(delayMs);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed {operationName} after {maxAttempts} attempts.");
                    throw;
                }
            }
        }

        private static string GenerateRandomNote(string definitionName)
        {
            var notes = new Dictionary<string, string[]>
            {
                { "WeighIn", new[] { "Stable weight", "Slight weight gain", "Weight loss observed" } },
                { "TempCheck", new[] { "Normal temperature", "Slightly elevated", "Fever detected" } },
                { "BehaviorNote", new[] { "Calm and relaxed", "Energetic and playful", "Slightly anxious" } },
                { "MedicationDose", new[] { "Administered on schedule", "Missed dose", "Adjusted dosage" } },
                { "LitterSize", new[] { "Healthy litter", "Small litter", "Large litter" } },
                { "HeartRate", new[] { "Normal range", "Elevated during exercise", "Low at rest" } },
                { "RespiratoryRate", new[] { "Normal breathing", "Rapid breathing post-exercise", "Slow and steady" } },
                { "HydrationStatus", new[] { "Well hydrated", "Mild dehydration", "Needs fluids" } },
                { "ExerciseDuration", new[] { "Completed full session", "Shortened due to fatigue", "Extended session" } },
                { "ExerciseIntensity", new[] { "Low intensity", "Moderate intensity", "High intensity" } },
                { "AppetiteLevel", new[] { "Good appetite", "Reduced appetite", "Very hungry" } },
                { "StoolQuality", new[] { "Normal consistency", "Soft stool", "Firm stool" } },
                { "DailyCaloricIntake", new[] { "Met caloric needs", "Underfed", "Overfed" } },
                { "SocializationBehavior", new[] { "Friendly with others", "Shy around strangers", "Playful with dogs" } },
                { "TrainingProgress", new[] { "Mastered command", "Needs practice", "Improving steadily" } },
                { "EstrusCycleStage", new[] { "Proestrus observed", "Estrus phase", "Diestrus phase" } },
                { "GestationProgress", new[] { "Early gestation", "Mid gestation", "Late gestation" } },
                { "CoatCondition", new[] { "Shiny and healthy", "Needs grooming", "Slight shedding" } },
                { "DentalHealth", new[] { "Clean teeth", "Mild tartar", "Needs dental check" } },
                { "PainAssessment", new[] { "No pain observed", "Mild discomfort", "Needs vet attention" } },
                { "AmbientHumidity", new[] { "Comfortable environment", "Too dry", "High humidity" } }
            };

            if (notes.TryGetValue(definitionName, out var noteOptions))
            {
                return noteOptions[new Random().Next(noteOptions.Length)];
            }

            return "General observation";
        }
    }
}