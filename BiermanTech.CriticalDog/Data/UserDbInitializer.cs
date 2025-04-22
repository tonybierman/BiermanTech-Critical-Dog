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
            var dogSubjectType = await context.SubjectTypes.FirstOrDefaultAsync(st => st.Name == "Dog");
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
                        CreatedAt = DateTime.UtcNow,
                        Permissions = 504
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
                        CreatedAt = DateTime.UtcNow,
                        Permissions = 504
                    }
                };

                context.AddRange(newSubjects);
                await RetryAsync(() => context.SaveChangesAsync(), logger, "Saving subjects");
                logger.LogInformation("Subjects seeded successfully.");
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

        private static string GenerateDeterministicNote(string definitionName)
        {
            var notes = new Dictionary<string, string>
            {
                { "WeighIn", "Stable weight" },
                { "TempCheck", "Normal temperature" },
                { "BehaviorNote", "Calm and relaxed" },
                { "MedicationDose", "Administered on schedule" },
                { "LitterSize", "Healthy litter" },
                { "HeartRate", "Normal range" },
                { "RespiratoryRate", "Normal breathing" },
                { "HydrationStatus", "Well hydrated" },
                { "ExerciseDuration", "Completed full session" },
                { "ExerciseIntensity", "Moderate intensity" },
                { "AppetiteLevel", "Good appetite" },
                { "StoolQuality", "Normal consistency" },
                { "DailyCaloricIntake", "Met caloric needs" },
                { "SocializationBehavior", "Friendly with others" },
                { "TrainingProgress", "Improving steadily" },
                { "EstrusCycleStage", "Proestrus observed" },
                { "GestationProgress", "Early gestation" },
                { "CoatCondition", "Shiny and healthy" },
                { "DentalHealth", "Clean teeth" },
                { "PainAssessment", "No pain observed" },
                { "AmbientHumidity", "Comfortable environment" }
            };

            return notes.TryGetValue(definitionName, out var note) ? note : "General observation";
        }
    }
}