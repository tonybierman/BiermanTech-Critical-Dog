using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Data
{
    public static class AppDbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, ILogger? logger = null)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            logger ??= serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Applying migrations for AppDbContext...");
                await context.Database.MigrateAsync();

                await SeedDataAsync(context, logger);

                logger.LogInformation("AppDbContext initialization completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing AppDbContext.");
                throw;
            }
        }

        private static async Task SeedDataAsync(AppDbContext context, ILogger logger)
        {
            // Seed Units
            if (!await context.Units.AnyAsync())
            {
                logger.LogInformation("Seeding Units...");
                context.Units.AddRange(
                    new Unit { UnitName = "Kilograms", UnitSymbol = "kg", Description = "Metric unit of mass", IsActive = true },
                    new Unit { UnitName = "Pounds", UnitSymbol = "lb", Description = "Imperial unit of mass", IsActive = true },
                    new Unit { UnitName = "Degrees Celsius", UnitSymbol = "°C", Description = "Metric unit of temperature", IsActive = true },
                    new Unit { UnitName = "Milliliters", UnitSymbol = "ml", Description = "Metric unit of volume", IsActive = true }
                );
                await context.SaveChangesAsync();
            }

            // Seed ObservationTypes
            if (!await context.ObservationTypes.AnyAsync())
            {
                logger.LogInformation("Seeding ObservationTypes...");
                context.ObservationTypes.AddRange(
                    new ObservationType { TypeName = "Weight", Description = "Measurement of mass", IsActive = true },
                    new ObservationType { TypeName = "Temperature", Description = "Measurement of body temperature", IsActive = true },
                    new ObservationType { TypeName = "Behavior", Description = "Observation of behavioral traits", IsActive = true },
                    new ObservationType { TypeName = "Medication", Description = "Administration of medication", IsActive = true }
                );
                await context.SaveChangesAsync();
            }

            // Seed ScientificDisciplines
            if (!await context.ScientificDisciplines.AnyAsync())
            {
                logger.LogInformation("Seeding ScientificDisciplines...");
                context.ScientificDisciplines.AddRange(
                    new ScientificDiscipline { DisciplineName = "Canine Biology", Description = "Study of dog anatomy, physiology, and genetics", IsActive = true },
                    new ScientificDiscipline { DisciplineName = "Nutrition Science", Description = "Study of dietary needs for canine health", IsActive = true },
                    new ScientificDiscipline { DisciplineName = "Veterinary Medicine", Description = "Diagnosis and treatment of canine diseases", IsActive = true },
                    new ScientificDiscipline { DisciplineName = "Ethology", Description = "Study of animal behavior", IsActive = true },
                    new ScientificDiscipline { DisciplineName = "Pharmacology", Description = "Study of drug effects on dogs", IsActive = true }
                );
                await context.SaveChangesAsync();
            }

            // Seed ObservationDefinitions
            if (!await context.ObservationDefinitions.AnyAsync())
            {
                logger.LogInformation("Seeding ObservationDefinitions...");
                var weightType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Weight");
                var tempType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Temperature");
                var behaviorType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Behavior");
                var medicationType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Medication");

                context.ObservationDefinitions.AddRange(
                    new ObservationDefinition
                    {
                        DefinitionName = "WeighIn",
                        ObservationTypeId = weightType.Id,
                        IsQualitative = false,
                        MinimumValue = 0m,
                        MaximumValue = 200m,
                        Description = "Quantitative measurement of subject weight",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "TempCheck",
                        ObservationTypeId = tempType.Id,
                        IsQualitative = false,
                        MinimumValue = 36m,
                        MaximumValue = 40m,
                        Description = "Quantitative measurement of body temperature",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "BehaviorNote",
                        ObservationTypeId = behaviorType.Id,
                        IsQualitative = true,
                        MinimumValue = null,
                        MaximumValue = null,
                        Description = "Qualitative note on subject behavior",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "MedicationDose",
                        ObservationTypeId = medicationType.Id,
                        IsQualitative = false,
                        MinimumValue = 0m,
                        MaximumValue = 1000m,
                        Description = "Quantitative record of medication administered",
                        IsActive = true
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed ObservationDefinitionDiscipline (junction table)
            if (!await context.Set<Dictionary<string, object>>("ObservationDefinitionDiscipline").AnyAsync())
            {
                logger.LogInformation("Seeding ObservationDefinitionDiscipline...");
                var weighIn = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "WeighIn");
                var tempCheck = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "TempCheck");
                var behaviorNote = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "BehaviorNote");
                var medicationDose = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "MedicationDose");

                var canineBiology = await context.ScientificDisciplines.FirstAsync(sd => sd.DisciplineName == "Canine Biology");
                var nutritionScience = await context.ScientificDisciplines.FirstAsync(sd => sd.DisciplineName == "Nutrition Science");
                var veterinaryMedicine = await context.ScientificDisciplines.FirstAsync(sd => sd.DisciplineName == "Veterinary Medicine");
                var ethology = await context.ScientificDisciplines.FirstAsync(sd => sd.DisciplineName == "Ethology");
                var pharmacology = await context.ScientificDisciplines.FirstAsync(sd => sd.DisciplineName == "Pharmacology");

                context.Set<Dictionary<string, object>>("ObservationDefinitionDiscipline").AddRange(
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", weighIn.Id },
                        { "ScientificDisciplineId", canineBiology.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", weighIn.Id },
                        { "ScientificDisciplineId", nutritionScience.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", tempCheck.Id },
                        { "ScientificDisciplineId", veterinaryMedicine.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", behaviorNote.Id },
                        { "ScientificDisciplineId", ethology.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", medicationDose.Id },
                        { "ScientificDisciplineId", veterinaryMedicine.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", medicationDose.Id },
                        { "ScientificDisciplineId", pharmacology.Id }
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed ObservationDefinitionUnit (junction table)
            if (!await context.Set<Dictionary<string, object>>("ObservationDefinitionUnit").AnyAsync())
            {
                logger.LogInformation("Seeding ObservationDefinitionUnit...");
                var weighIn = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "WeighIn");
                var tempCheck = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "TempCheck");
                var medicationDose = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "MedicationDose");

                var kilograms = await context.Units.FirstAsync(u => u.UnitName == "Kilograms");
                var pounds = await context.Units.FirstAsync(u => u.UnitName == "Pounds");
                var celsius = await context.Units.FirstAsync(u => u.UnitName == "Degrees Celsius");
                var milliliters = await context.Units.FirstAsync(u => u.UnitName == "Milliliters");

                context.Set<Dictionary<string, object>>("ObservationDefinitionUnit").AddRange(
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", weighIn.Id },
                        { "UnitId", kilograms.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", weighIn.Id },
                        { "UnitId", pounds.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", tempCheck.Id },
                        { "UnitId", celsius.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", medicationDose.Id },
                        { "UnitId", milliliters.Id }
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed MetricTypes
            if (!await context.MetricTypes.AnyAsync())
            {
                logger.LogInformation("Seeding MetricTypes...");
                var weighIn = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "WeighIn");
                var tempCheck = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "TempCheck");
                var medicationDose = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "MedicationDose");

                var kilograms = await context.Units.FirstAsync(u => u.UnitName == "Kilograms");
                var pounds = await context.Units.FirstAsync(u => u.UnitName == "Pounds");
                var celsius = await context.Units.FirstAsync(u => u.UnitName == "Degrees Celsius");
                var milliliters = await context.Units.FirstAsync(u => u.UnitName == "Milliliters");

                context.MetricTypes.AddRange(
                    new MetricType
                    {
                        ObservationDefinitionId = weighIn.Id,
                        UnitId = kilograms.Id,
                        Description = "Weight in kilograms",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = weighIn.Id,
                        UnitId = pounds.Id,
                        Description = "Weight in pounds",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = tempCheck.Id,
                        UnitId = celsius.Id,
                        Description = "Temperature in Celsius",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = medicationDose.Id,
                        UnitId = milliliters.Id,
                        Description = "Medication dose in milliliters",
                        IsActive = true
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed MetaTags
            if (!await context.MetaTags.AnyAsync())
            {
                logger.LogInformation("Seeding MetaTags...");
                context.MetaTags.AddRange(
                    new MetaTag { TagName = "Feeding", Description = "Related to food or feeding observations", IsActive = true },
                    new MetaTag { TagName = "Medication", Description = "Indicates a medication-related record", IsActive = true },
                    new MetaTag { TagName = "Exercise", Description = "Pertains to physical activity or exercise", IsActive = true },
                    new MetaTag { TagName = "Grooming", Description = "Related to grooming or hygiene", IsActive = true },
                    new MetaTag { TagName = "Health Check", Description = "General health or veterinary check-up", IsActive = true },
                    new MetaTag { TagName = "Behavior", Description = "Observations about behavior or temperament", IsActive = true }
                );
                await context.SaveChangesAsync();
            }

            // Seed SubjectTypes
            if (!await context.SubjectTypes.AnyAsync())
            {
                logger.LogInformation("Seeding SubjectTypes...");
                context.SubjectTypes.AddRange(
                    new SubjectType() { TypeName = "Dog", Description = "The dog (Canis familiaris or Canis lupus familiaris) is a domesticated descendant of the gray wolf." }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}