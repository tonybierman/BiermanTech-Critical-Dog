using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
                    // Mass Units (for weight, feed, birth weight)
                    new Unit { UnitName = "Kilograms", UnitSymbol = "kg", Description = "Metric unit of mass", IsActive = true },
                    new Unit { UnitName = "Pounds", UnitSymbol = "lb", Description = "Imperial unit of mass", IsActive = true },
                    new Unit { UnitName = "Ounces", UnitSymbol = "oz", Description = "Imperial unit of mass", IsActive = true },
                    new Unit { UnitName = "Grams", UnitSymbol = "g", Description = "Metric unit of mass", IsActive = true },
                    new Unit { UnitName = "Milligrams", UnitSymbol = "mg", Description = "Metric unit of mass", IsActive = true },

                    // Volume Units (for fluids, feed, cleaning solutions)
                    new Unit { UnitName = "Milliliters", UnitSymbol = "mL", Description = "Metric unit of volume", IsActive = true },
                    new Unit { UnitName = "Liters", UnitSymbol = "L", Description = "Metric unit of volume", IsActive = true },
                    new Unit { UnitName = "Cups", UnitSymbol = "c", Description = "US customary unit of volume", IsActive = true },
                    new Unit { UnitName = "Fluid Ounces", UnitSymbol = "fl oz", Description = "US customary unit of volume", IsActive = true },

                    // Energy Units (for nutrition, feed energy)
                    new Unit { UnitName = "Kilocalories", UnitSymbol = "kcal", Description = "Metric unit of energy", IsActive = true },
                    new Unit { UnitName = "Calories", UnitSymbol = "cal", Description = "Metric unit of energy", IsActive = true },
                    new Unit { UnitName = "Kilojoules", UnitSymbol = "kJ", Description = "Metric unit of energy", IsActive = true },

                    // Temperature Units (for body temperature, environment)
                    new Unit { UnitName = "Degrees Celsius", UnitSymbol = "°C", Description = "Metric unit of temperature", IsActive = true },
                    new Unit { UnitName = "Degrees Fahrenheit", UnitSymbol = "°F", Description = "Imperial unit of temperature", IsActive = true },

                    // Length Units (for body measurements)
                    new Unit { UnitName = "Centimeters", UnitSymbol = "cm", Description = "Metric unit of length", IsActive = true },
                    new Unit { UnitName = "Inches", UnitSymbol = "in", Description = "Imperial unit of length", IsActive = true },

                    // Time Units (for gestation, schedules, exercise, age)
                    new Unit { UnitName = "Minutes", UnitSymbol = "min", Description = "Unit of time", IsActive = true },
                    new Unit { UnitName = "Days", UnitSymbol = "d", Description = "Unit of time", IsActive = true },
                    new Unit { UnitName = "Hours", UnitSymbol = "h", Description = "Unit of time", IsActive = true },
                    new Unit { UnitName = "Weeks", UnitSymbol = "wk", Description = "Unit of time", IsActive = true },
                    new Unit { UnitName = "Years", UnitSymbol = "yr", Description = "Unit of time", IsActive = true },

                    // Area Unit (for kennel or exercise space)
                    new Unit { UnitName = "Square Meters", UnitSymbol = "m²", Description = "Metric unit of area", IsActive = true },

                    // Specialized Units (for medications, vitamins)
                    new Unit { UnitName = "International Units", UnitSymbol = "IU", Description = "Unit for biological activity (e.g., vitamins)", IsActive = true },

                    // Percentage (for humidity, health metrics)
                    new Unit { UnitName = "Percentage", UnitSymbol = "%", Description = "Unit of proportion or ratio", IsActive = true },

                    // Quantity Unit (for offspring, items)
                    new Unit { UnitName = "Count", UnitSymbol = "ct", Description = "Denotes a quantity or number of items", IsActive = true },

                    // New Units
                    new Unit { UnitName = "Beats per Minute", UnitSymbol = "bpm", Description = "Unit for heart rate or respiratory rate", IsActive = true },
                    new Unit { UnitName = "Score", UnitSymbol = "scr", Description = "Unit for semi-quantitative scales (e.g., 1–10)", IsActive = true }
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
                    new ObservationType { TypeName = "Medication", Description = "Administration of medication", IsActive = true },
                    new ObservationType { TypeName = "CanineLifeStageFactor", Description = "Use to determine energy requirement", IsActive = true },
                    new ObservationType { TypeName = "CanineOfaHipGrade", Description = "Grading system used by OFA for radiographic hips evaluation", IsActive = true },
                    new ObservationType { TypeName = "CanineGeneticHealthConditionStatus", Description = "Covers the possible outcomes for canine genetic health conditions", IsActive = true },
                    new ObservationType { TypeName = "mtDNA", Description = "mitochondrial DNA", IsActive = true },
                    new ObservationType { TypeName = "Y-DNA", Description = "Y-chromosomal DNA", IsActive = true },
                    new ObservationType { TypeName = "Reproduction", Description = "Of or related to the biological process of producing offspring", IsActive = true },
                    // New Observation Types
                    new ObservationType { TypeName = "Vital Signs", Description = "Measurement of physiological vital signs", IsActive = true },
                    new ObservationType { TypeName = "Exercise", Description = "Observation of physical activity", IsActive = true },
                    new ObservationType { TypeName = "Nutrition", Description = "Observation of dietary intake or digestion", IsActive = true },
                    new ObservationType { TypeName = "Grooming", Description = "Observation of coat, skin, or dental condition", IsActive = true },
                    new ObservationType { TypeName = "Environment", Description = "Observation of environmental conditions", IsActive = true }
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
                var reproType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Reproduction");
                var vitalSignsType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Vital Signs");
                var exerciseType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Exercise");
                var nutritionType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Nutrition");
                var groomingType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Grooming");
                var environmentType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Environment");

                context.ObservationDefinitions.AddRange(
                    // Existing Definitions
                    new ObservationDefinition
                    {
                        DefinitionName = "Weigh In",
                        ObservationTypeId = weightType.Id,
                        IsQualitative = false,
                        MinimumValue = 0m,
                        MaximumValue = 200m,
                        Description = "Quantitative measurement of subject weight",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Temp Check",
                        ObservationTypeId = tempType.Id,
                        IsQualitative = false,
                        MinimumValue = 36m,
                        MaximumValue = 40m,
                        Description = "Quantitative measurement of body temperature",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Behavior Note",
                        ObservationTypeId = behaviorType.Id,
                        IsQualitative = true,
                        MinimumValue = null,
                        MaximumValue = null,
                        Description = "Qualitative note on subject behavior",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Medication Dose",
                        ObservationTypeId = medicationType.Id,
                        IsQualitative = false,
                        MinimumValue = 0m,
                        MaximumValue = 1000m,
                        Description = "Quantitative record of medication administered",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Litter Size",
                        ObservationTypeId = reproType.Id,
                        IsQualitative = false,
                        MinimumValue = 0m,
                        MaximumValue = 24m,
                        Description = "Count of offspring",
                        IsActive = true
                    },
                    // New Definitions
                    new ObservationDefinition
                    {
                        DefinitionName = "Heart Rate",
                        ObservationTypeId = vitalSignsType.Id,
                        IsQualitative = false,
                        MinimumValue = 40m,
                        MaximumValue = 180m,
                        Description = "Quantitative measurement of heart rate",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Respiratory Rate",
                        ObservationTypeId = vitalSignsType.Id,
                        IsQualitative = false,
                        MinimumValue = 10m,
                        MaximumValue = 60m,
                        Description = "Quantitative measurement of respiratory rate",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Hydration Status",
                        ObservationTypeId = vitalSignsType.Id,
                        IsQualitative = false,
                        MinimumValue = 0m,
                        MaximumValue = 100m,
                        Description = "Quantitative estimate of hydration level",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Exercise Duration",
                        ObservationTypeId = exerciseType.Id,
                        IsQualitative = false,
                        MinimumValue = 0m,
                        MaximumValue = 300m,
                        Description = "Quantitative measurement of exercise time",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Exercise Intensity",
                        ObservationTypeId = exerciseType.Id,
                        IsQualitative = true,
                        MinimumValue = null,
                        MaximumValue = null,
                        Description = "Qualitative assessment of exercise intensity (low, moderate, high)",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Appetite Level",
                        ObservationTypeId = nutritionType.Id,
                        IsQualitative = true,
                        MinimumValue = null,
                        MaximumValue = null,
                        Description = "Qualitative observation of food intake",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Stool Quality",
                        ObservationTypeId = nutritionType.Id,
                        IsQualitative = false,
                        MinimumValue = 1m,
                        MaximumValue = 7m,
                        Description = "Semi-quantitative score of stool consistency (1–7 scale)",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Daily Caloric Intake",
                        ObservationTypeId = nutritionType.Id,
                        IsQualitative = false,
                        MinimumValue = 0m,
                        MaximumValue = 5000m,
                        Description = "Quantitative measurement of daily energy intake",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Socialization Behavior",
                        ObservationTypeId = behaviorType.Id,
                        IsQualitative = true,
                        MinimumValue = null,
                        MaximumValue = null,
                        Description = "Qualitative note on socialization with dogs or humans",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Training Progress",
                        ObservationTypeId = behaviorType.Id,
                        IsQualitative = false,
                        MinimumValue = 1m,
                        MaximumValue = 5m,
                        Description = "Semi-quantitative score of training command mastery",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Estrus Cycle Stage",
                        ObservationTypeId = reproType.Id,
                        IsQualitative = true,
                        MinimumValue = null,
                        MaximumValue = null,
                        Description = "Qualitative observation of reproductive cycle phase",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Gestation Progress",
                        ObservationTypeId = reproType.Id,
                        IsQualitative = false,
                        MinimumValue = 0m,
                        MaximumValue = 70m,
                        Description = "Quantitative measurement of gestation time in days",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Coat Condition",
                        ObservationTypeId = groomingType.Id,
                        IsQualitative = false,
                        MinimumValue = 1m,
                        MaximumValue = 5m,
                        Description = "Semi-quantitative score of coat health",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Dental Health",
                        ObservationTypeId = groomingType.Id,
                        IsQualitative = false,
                        MinimumValue = 1m,
                        MaximumValue = 5m,
                        Description = "Semi-quantitative score of dental condition",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Pain Assessment",
                        ObservationTypeId = vitalSignsType.Id,
                        IsQualitative = false,
                        MinimumValue = 1m,
                        MaximumValue = 10m,
                        Description = "Semi-quantitative score of pain level",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Ambient Humidity",
                        ObservationTypeId = environmentType.Id,
                        IsQualitative = false,
                        MinimumValue = 0m,
                        MaximumValue = 100m,
                        Description = "Quantitative measurement of environmental humidity",
                        IsActive = true
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed ObservationDefinitionDiscipline
            if (!await context.Set<Dictionary<string, object>>("ObservationDefinitionDiscipline").AnyAsync())
            {
                logger.LogInformation("Seeding ObservationDefinitionDiscipline...");
                var weighIn = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Weigh In");
                var tempCheck = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Temp Check");
                var behaviorNote = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Behavior Note");
                var medicationDose = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Medication Dose");
                var litterSize = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Litter Size");
                var heartRate = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Heart Rate");
                var respiratoryRate = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Respiratory Rate");
                var hydrationStatus = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Hydration Status");
                var exerciseDuration = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Exercise Duration");
                var exerciseIntensity = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Exercise Intensity");
                var appetiteLevel = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Appetite Level");
                var stoolQuality = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Stool Quality");
                var dailyCaloricIntake = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Daily Caloric Intake");
                var socializationBehavior = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Socialization Behavior");
                var trainingProgress = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Training Progress");
                var estrusCycleStage = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Estrus Cycle Stage");
                var gestationProgress = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Gestation Progress");
                var coatCondition = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Coat Condition");
                var dentalHealth = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Dental Health");
                var painAssessment = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Pain Assessment");
                var ambientHumidity = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Ambient Humidity");

                var canineBiology = await context.ScientificDisciplines.FirstAsync(sd => sd.DisciplineName == "Canine Biology");
                var nutritionScience = await context.ScientificDisciplines.FirstAsync(sd => sd.DisciplineName == "Nutrition Science");
                var veterinaryMedicine = await context.ScientificDisciplines.FirstAsync(sd => sd.DisciplineName == "Veterinary Medicine");
                var ethology = await context.ScientificDisciplines.FirstAsync(sd => sd.DisciplineName == "Ethology");
                var pharmacology = await context.ScientificDisciplines.FirstAsync(sd => sd.DisciplineName == "Pharmacology");

                context.Set<Dictionary<string, object>>("ObservationDefinitionDiscipline").AddRange(
                    // Existing Entries
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
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", litterSize.Id },
                        { "ScientificDisciplineId", canineBiology.Id }
                    },
                    // New Entries
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", heartRate.Id },
                        { "ScientificDisciplineId", veterinaryMedicine.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", respiratoryRate.Id },
                        { "ScientificDisciplineId", veterinaryMedicine.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", hydrationStatus.Id },
                        { "ScientificDisciplineId", veterinaryMedicine.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", exerciseDuration.Id },
                        { "ScientificDisciplineId", canineBiology.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", exerciseIntensity.Id },
                        { "ScientificDisciplineId", ethology.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", appetiteLevel.Id },
                        { "ScientificDisciplineId", nutritionScience.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", stoolQuality.Id },
                        { "ScientificDisciplineId", veterinaryMedicine.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", dailyCaloricIntake.Id },
                        { "ScientificDisciplineId", nutritionScience.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", socializationBehavior.Id },
                        { "ScientificDisciplineId", ethology.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", trainingProgress.Id },
                        { "ScientificDisciplineId", ethology.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", estrusCycleStage.Id },
                        { "ScientificDisciplineId", canineBiology.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", gestationProgress.Id },
                        { "ScientificDisciplineId", canineBiology.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", coatCondition.Id },
                        { "ScientificDisciplineId", veterinaryMedicine.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", dentalHealth.Id },
                        { "ScientificDisciplineId", veterinaryMedicine.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", painAssessment.Id },
                        { "ScientificDisciplineId", veterinaryMedicine.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", ambientHumidity.Id },
                        { "ScientificDisciplineId", canineBiology.Id }
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed ObservationDefinitionUnit
            if (!await context.Set<Dictionary<string, object>>("ObservationDefinitionUnit").AnyAsync())
            {
                logger.LogInformation("Seeding ObservationDefinitionUnit...");
                var weighIn = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Weigh In");
                var tempCheck = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Temp Check");
                var medicationDose = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Medication Dose");
                var litterSize = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Litter Size");
                var heartRate = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Heart Rate");
                var respiratoryRate = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Respiratory Rate");
                var hydrationStatus = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Hydration Status");
                var exerciseDuration = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Exercise Duration");
                var stoolQuality = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Stool Quality");
                var dailyCaloricIntake = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Daily Caloric Intake");
                var trainingProgress = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Training Progress");
                var gestationProgress = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Gestation Progress");
                var coatCondition = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Coat Condition");
                var dentalHealth = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Dental Health");
                var painAssessment = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Pain Assessment");
                var ambientHumidity = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Ambient Humidity");

                var kilograms = await context.Units.FirstAsync(u => u.UnitName == "Kilograms");
                var pounds = await context.Units.FirstAsync(u => u.UnitName == "Pounds");
                var ounces = await context.Units.FirstAsync(u => u.UnitName == "Ounces");
                var celsius = await context.Units.FirstAsync(u => u.UnitName == "Degrees Celsius");
                var fahrenheit = await context.Units.FirstAsync(u => u.UnitName == "Degrees Fahrenheit");
                var milliliters = await context.Units.FirstAsync(u => u.UnitName == "Milliliters");
                var milligrams = await context.Units.FirstAsync(u => u.UnitName == "Milligrams");
                var grams = await context.Units.FirstAsync(u => u.UnitName == "Grams");
                var count = await context.Units.FirstAsync(u => u.UnitName == "Count");
                var bpm = await context.Units.FirstAsync(u => u.UnitName == "Beats per Minute");
                var percentage = await context.Units.FirstAsync(u => u.UnitName == "Percentage");
                var minutes = await context.Units.FirstAsync(u => u.UnitName == "Minutes");
                var score = await context.Units.FirstAsync(u => u.UnitName == "Score");
                var kilocalories = await context.Units.FirstAsync(u => u.UnitName == "Kilocalories");
                var days = await context.Units.FirstAsync(u => u.UnitName == "Days");

                context.Set<Dictionary<string, object>>("ObservationDefinitionUnit").AddRange(
                    // Existing Entries
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", weighIn.Id },
                        { "UnitId", kilograms.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", weighIn.Id },
                        { "UnitId", grams.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", weighIn.Id },
                        { "UnitId", milligrams.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", weighIn.Id },
                        { "UnitId", pounds.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", weighIn.Id },
                        { "UnitId", ounces.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", tempCheck.Id },
                        { "UnitId", celsius.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", tempCheck.Id },
                        { "UnitId", fahrenheit.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", medicationDose.Id },
                        { "UnitId", milliliters.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", medicationDose.Id },
                        { "UnitId", milligrams.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", medicationDose.Id },
                        { "UnitId", grams.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", medicationDose.Id },
                        { "UnitId", count.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", litterSize.Id },
                        { "UnitId", count.Id }
                    },
                    // New Entries
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", heartRate.Id },
                        { "UnitId", bpm.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", respiratoryRate.Id },
                        { "UnitId", bpm.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", hydrationStatus.Id },
                        { "UnitId", percentage.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", exerciseDuration.Id },
                        { "UnitId", minutes.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", stoolQuality.Id },
                        { "UnitId", score.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", dailyCaloricIntake.Id },
                        { "UnitId", kilocalories.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", trainingProgress.Id },
                        { "UnitId", score.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", gestationProgress.Id },
                        { "UnitId", days.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", coatCondition.Id },
                        { "UnitId", score.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", dentalHealth.Id },
                        { "UnitId", score.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", painAssessment.Id },
                        { "UnitId", score.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", ambientHumidity.Id },
                        { "UnitId", percentage.Id }
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed MetricTypes
            if (!await context.MetricTypes.AnyAsync())
            {
                logger.LogInformation("Seeding MetricTypes...");
                var weighIn = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Weigh In");
                var tempCheck = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Temp Check");
                var medicationDose = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Medication Dose");
                var litterSize = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Litter Size");
                var heartRate = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Heart Rate");
                var respiratoryRate = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Respiratory Rate");
                var hydrationStatus = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Hydration Status");
                var exerciseDuration = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Exercise Duration");
                var stoolQuality = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Stool Quality");
                var dailyCaloricIntake = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Daily Caloric Intake");
                var trainingProgress = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Training Progress");
                var gestationProgress = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Gestation Progress");
                var coatCondition = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Coat Condition");
                var dentalHealth = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Dental Health");
                var painAssessment = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Pain Assessment");
                var ambientHumidity = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Ambient Humidity");

                var kilograms = await context.Units.FirstAsync(u => u.UnitName == "Kilograms");
                var pounds = await context.Units.FirstAsync(u => u.UnitName == "Pounds");
                var ounces = await context.Units.FirstAsync(u => u.UnitName == "Ounces");
                var celsius = await context.Units.FirstAsync(u => u.UnitName == "Degrees Celsius");
                var fahrenheit = await context.Units.FirstAsync(u => u.UnitName == "Degrees Fahrenheit");
                var milliliters = await context.Units.FirstAsync(u => u.UnitName == "Milliliters");
                var milligrams = await context.Units.FirstAsync(u => u.UnitName == "Milligrams");
                var grams = await context.Units.FirstAsync(u => u.UnitName == "Grams");
                var count = await context.Units.FirstAsync(u => u.UnitName == "Count");
                var bpm = await context.Units.FirstAsync(u => u.UnitName == "Beats per Minute");
                var percentage = await context.Units.FirstAsync(u => u.UnitName == "Percentage");
                var minutes = await context.Units.FirstAsync(u => u.UnitName == "Minutes");
                var score = await context.Units.FirstAsync(u => u.UnitName == "Score");
                var kilocalories = await context.Units.FirstAsync(u => u.UnitName == "Kilocalories");
                var days = await context.Units.FirstAsync(u => u.UnitName == "Days");

                context.MetricTypes.AddRange(
                    // Existing Entries
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
                        UnitId = milligrams.Id,
                        Description = "Weight in milligrams",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = weighIn.Id,
                        UnitId = grams.Id,
                        Description = "Weight in grams",
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
                        ObservationDefinitionId = weighIn.Id,
                        UnitId = ounces.Id,
                        Description = "Weight in ounces",
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
                        ObservationDefinitionId = tempCheck.Id,
                        UnitId = fahrenheit.Id,
                        Description = "Temperature in Fahrenheit",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = medicationDose.Id,
                        UnitId = milliliters.Id,
                        Description = "Medication dose in milliliters",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = medicationDose.Id,
                        UnitId = milligrams.Id,
                        Description = "Medication dose in milligrams",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = medicationDose.Id,
                        UnitId = grams.Id,
                        Description = "Medication dose in grams",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = medicationDose.Id,
                        UnitId = count.Id,
                        Description = "Medication dose count",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = litterSize.Id,
                        UnitId = count.Id,
                        Description = "Count of offspring",
                        IsActive = true
                    },
                    // New Entries
                    new MetricType
                    {
                        ObservationDefinitionId = heartRate.Id,
                        UnitId = bpm.Id,
                        Description = "Heart rate in beats per minute",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = respiratoryRate.Id,
                        UnitId = bpm.Id,
                        Description = "Respiratory rate in breaths per minute",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = hydrationStatus.Id,
                        UnitId = percentage.Id,
                        Description = "Hydration level in percentage",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = exerciseDuration.Id,
                        UnitId = minutes.Id,
                        Description = "Exercise duration in minutes",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = stoolQuality.Id,
                        UnitId = score.Id,
                        Description = "Stool quality score (1–7)",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = dailyCaloricIntake.Id,
                        UnitId = kilocalories.Id,
                        Description = "Daily caloric intake in kilocalories",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = trainingProgress.Id,
                        UnitId = score.Id,
                        Description = "Training progress score (1–5)",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = gestationProgress.Id,
                        UnitId = days.Id,
                        Description = "Gestation progress in days",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = coatCondition.Id,
                        UnitId = score.Id,
                        Description = "Coat condition score (1–5)",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = dentalHealth.Id,
                        UnitId = score.Id,
                        Description = "Dental health score (1–5)",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = painAssessment.Id,
                        UnitId = score.Id,
                        Description = "Pain assessment score (1–10)",
                        IsActive = true
                    },
                    new MetricType
                    {
                        ObservationDefinitionId = ambientHumidity.Id,
                        UnitId = percentage.Id,
                        Description = "Ambient humidity in percentage",
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
                    // Existing Entries
                    new MetaTag { TagName = "Feeding", Description = "Related to food or feeding observations", IsActive = true },
                    new MetaTag { TagName = "Medication", Description = "Indicates a medication-related record", IsActive = true },
                    new MetaTag { TagName = "Exercise", Description = "Pertains to physical activity or exercise", IsActive = true },
                    new MetaTag { TagName = "Grooming", Description = "Related to grooming or hygiene", IsActive = true },
                    new MetaTag { TagName = "Health Check", Description = "General health or veterinary check-up", IsActive = true },
                    new MetaTag { TagName = "Behavior", Description = "Observations about behavior or temperament", IsActive = true },
                    // New Entries
                    new MetaTag { TagName = "Vital Signs", Description = "Related to physiological measurements", IsActive = true },
                    new MetaTag { TagName = "Reproduction", Description = "Related to reproductive observations", IsActive = true },
                    new MetaTag { TagName = "Training", Description = "Related to training or obedience observations", IsActive = true },
                    new MetaTag { TagName = "Environment", Description = "Related to environmental conditions", IsActive = true }
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