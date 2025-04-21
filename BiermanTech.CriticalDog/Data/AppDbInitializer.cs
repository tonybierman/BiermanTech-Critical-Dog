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
                    new Unit { UnitName = "FluidOunces", UnitSymbol = "fl oz", Description = "US customary unit of volume", IsActive = true },

                    // Energy Units (for nutrition, feed energy)
                    new Unit { UnitName = "Kilocalories", UnitSymbol = "kcal", Description = "Metric unit of energy", IsActive = true },
                    new Unit { UnitName = "Calories", UnitSymbol = "cal", Description = "Metric unit of energy", IsActive = true },
                    new Unit { UnitName = "Kilojoules", UnitSymbol = "kJ", Description = "Metric unit of energy", IsActive = true },

                    // Temperature Units (for body temperature, environment)
                    new Unit { UnitName = "DegreesCelsius", UnitSymbol = "°C", Description = "Metric unit of temperature", IsActive = true },
                    new Unit { UnitName = "DegreesFahrenheit", UnitSymbol = "°F", Description = "Imperial unit of temperature", IsActive = true },

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
                    new Unit { UnitName = "SquareMeters", UnitSymbol = "m²", Description = "Metric unit of area", IsActive = true },

                    // Specialized Units (for medications, vitamins)
                    new Unit { UnitName = "InternationalUnits", UnitSymbol = "IU", Description = "Unit for biological activity (e.g., vitamins)", IsActive = true },

                    // Percentage (for humidity, health metrics)
                    new Unit { UnitName = "Percentage", UnitSymbol = "%", Description = "Unit of proportion or ratio", IsActive = true },

                    // Quantity Unit (for offspring, items)
                    new Unit { UnitName = "Count", UnitSymbol = "ct", Description = "Denotes a quantity or number of items", IsActive = true },

                    // New Units
                    new Unit { UnitName = "BeatsPerMinute", UnitSymbol = "bpm", Description = "Unit for heart rate or respiratory rate", IsActive = true },
                    new Unit { UnitName = "Score", UnitSymbol = "scr", Description = "Unit for semi-quantitative scales (e.g., 1–10)", IsActive = true }
                );
                await context.SaveChangesAsync();
            }

            // Seed ObservationTypes
            // An observation type dictates the units of measure that can be applied
            if (!await context.ObservationTypes.AnyAsync())
            {
                logger.LogInformation("Seeding ObservationTypes...");
                context.ObservationTypes.AddRange(
                    new ObservationType { TypeName = "Qualitative", Description = "Descriptive, often categorical and discrete", IsActive = true },
                    new ObservationType { TypeName = "Weight", Description = "Measurement of mass", IsActive = true },
                    new ObservationType { TypeName = "Cohort", Description = "Number of members in a cohort", IsActive = true },
                    new ObservationType { TypeName = "EstrusStage", Description = "Defines the stages of an estrus cycle", IsActive = true },
                    new ObservationType { TypeName = "Temperature", Description = "Measurement of body temperature", IsActive = true },
                    new ObservationType { TypeName = "TemporalDays", Description = "Measurement of time in days", IsActive = true },
                    new ObservationType { TypeName = "Behavior", Description = "Observation of behavioral traits", IsActive = true },
                    new ObservationType { TypeName = "Medication", Description = "Administration of medication", IsActive = true },
                    new ObservationType { TypeName = "GeneticHealthConditionStatus", Description = "Covers the possible outcomes for a canine genetic health condition", IsActive = true },
                    new ObservationType { TypeName = "OfaHipsGrade", Description = "OFA radiographic evaluation of canine hip joint structure", IsActive = true },
                    new ObservationType { TypeName = "VitalSigns", Description = "Measurement of physiological vital signs", IsActive = true },
                    new ObservationType { TypeName = "ExerciseDuration", Description = "Observation of physical activity", IsActive = true },
                    new ObservationType { TypeName = "ExerciseIntensity", Description = "Observation of physical activity", IsActive = true },
                    new ObservationType { TypeName = "Nutrition", Description = "Observation of dietary intake or digestion", IsActive = true },
                    new ObservationType { TypeName = "LifeStageFactor", Description = "Used in nutrition to determine energy requirements", IsActive = true },
                    new ObservationType { TypeName = "Grooming", Description = "Observation of coat, skin, or dental condition", IsActive = true },
                    new ObservationType { TypeName = "Environment", Description = "Observation of environmental conditions", IsActive = true },
                    new ObservationType { TypeName = "StoolQualityScore", Description = "Score of stool consistency (1–7 scale)", IsActive = true }
                );
                await context.SaveChangesAsync();
            }

            // Seed ScientificDisciplines
            if (!await context.ScientificDisciplines.AnyAsync())
            {
                logger.LogInformation("Seeding ScientificDisciplines...");
                context.ScientificDisciplines.AddRange(
                    new ScientificDiscipline { DisciplineName = "CanineBiology", Description = "Study of dog anatomy, physiology, and genetics", IsActive = true },
                    new ScientificDiscipline { DisciplineName = "NutritionScience", Description = "Study of dietary needs for canine health", IsActive = true },
                    new ScientificDiscipline { DisciplineName = "VeterinaryMedicine", Description = "Diagnosis and treatment of canine diseases", IsActive = true },
                    new ScientificDiscipline { DisciplineName = "Ethology", Description = "Study of animal behavior", IsActive = true },
                    new ScientificDiscipline { DisciplineName = "Pharmacology", Description = "Study of drug effects on dogs", IsActive = true }
                );
                await context.SaveChangesAsync();
            }

            // Seed ObservationDefinitions
            if (!await context.ObservationDefinitions.AnyAsync())
            {
                logger.LogInformation("Seeding ObservationDefinitions...");
                var qualitativeType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Qualitative");
                var weightType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Weight");
                var cohortType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Cohort");
                var estrusStageType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "EstrusStage");
                var tempType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Temperature");
                var timeDaysType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "TemporalDays");
                var behaviorType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Behavior");
                var medicationType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Medication");
                var vitalSignsType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "VitalSigns");
                var exerciseDurationType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "ExerciseDuration");
                var exerciseIntensityType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "ExerciseIntensity");
                var geneticConditionType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "GeneticHealthConditionStatus");
                var ofaHipsGradeType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "OfaHipsGrade");
                var nutritionType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Nutrition");
                var lifeStageFactorType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "LifeStageFactor");
                var groomingType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Grooming");
                var environmentType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "Environment");
                var stoolQualityType = await context.ObservationTypes.FirstAsync(ot => ot.TypeName == "StoolQualityScore");

                context.ObservationDefinitions.AddRange(
                    new ObservationDefinition
                    {
                        DefinitionName = "mtDNA",
                        ObservationTypeId = qualitativeType.Id,
                        Description = "Record of mitochondrial DNA haplotype or haplogroup for maternal lineage analysis",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "Y-DNA",
                        ObservationTypeId = qualitativeType.Id,
                        Description = "Record of Y-chromosome DNA haplotype or haplogroup for paternal lineage analysis",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "CanineGeneticHealthConditionStatus",
                        ObservationTypeId = geneticConditionType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 4m,
                        Description = "Covers the possible outcomes for a canine genetic health condition",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "CanineOfaHipGrade",
                        ObservationTypeId = ofaHipsGradeType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 7m,
                        Description = "OFA radiographic evaluation of canine hip joint structure",
                        IsActive = true,
                        IsSingular = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "WeighIn",
                        ObservationTypeId = weightType.Id,
                        MinimumValue = 0m,
                        MaximumValue = 200m,
                        Description = "Quantitative measurement of subject weight",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "TempCheck",
                        ObservationTypeId = tempType.Id,
                        MinimumValue = 36m,
                        MaximumValue = 40m,
                        Description = "Quantitative measurement of body temperature",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "BehaviorNote",
                        ObservationTypeId = behaviorType.Id,
                        MinimumValue = null,
                        MaximumValue = null,
                        Description = "Qualitative note on subject behavior",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "MedicationDose",
                        ObservationTypeId = medicationType.Id,
                        MinimumValue = 0m,
                        MaximumValue = 1000m,
                        Description = "Quantitative record of medication administered",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "LitterSize",
                        ObservationTypeId = cohortType.Id,
                        MinimumValue = 0m,
                        MaximumValue = 24m,
                        Description = "Count of offspring",
                        IsActive = true
                    },
                    // New Definitions
                    new ObservationDefinition
                    {
                        DefinitionName = "HeartRate",
                        ObservationTypeId = vitalSignsType.Id,
                        MinimumValue = 40m,
                        MaximumValue = 180m,
                        Description = "Quantitative measurement of heart rate",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "RespiratoryRate",
                        ObservationTypeId = vitalSignsType.Id,
                        MinimumValue = 10m,
                        MaximumValue = 60m,
                        Description = "Quantitative measurement of respiratory rate",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "HydrationStatus",
                        ObservationTypeId = vitalSignsType.Id,
                        MinimumValue = 0m,
                        MaximumValue = 100m,
                        Description = "Quantitative estimate of hydration level",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "ExerciseDuration",
                        ObservationTypeId = exerciseDurationType.Id,
                        MinimumValue = 0m,
                        MaximumValue = 300m,
                        Description = "Quantitative measurement of exercise time",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "ExerciseIntensity",
                        ObservationTypeId = exerciseIntensityType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 4m,
                        Description = "Qualitative assessment of exercise intensity (low, moderate, high)",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "AppetiteLevel",
                        ObservationTypeId = nutritionType.Id,
                        MinimumValue = null,
                        MaximumValue = null,
                        Description = "Qualitative observation of food intake",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "CanineLifeStageFactor",
                        ObservationTypeId = lifeStageFactorType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 10m,
                        Description = "Used in nutrition to determine energy requirements",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "StoolQuality",
                        ObservationTypeId = stoolQualityType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 7m,
                        Description = "Score of stool consistency (1–7 scale)",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "DailyCaloricIntake",
                        ObservationTypeId = nutritionType.Id,
                        MinimumValue = 0m,
                        MaximumValue = 5000m,
                        Description = "Quantitative measurement of daily energy intake",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "SocializationBehavior",
                        ObservationTypeId = behaviorType.Id,
                        MinimumValue = null,
                        MaximumValue = null,
                        Description = "Qualitative note on socialization with dogs or humans",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "TrainingProgress",
                        ObservationTypeId = behaviorType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 5m,
                        Description = "Semi-quantitative score of training command mastery",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "EstrusCycleStage",
                        ObservationTypeId = estrusStageType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 4m,
                        Description = "Qualitative observation of reproductive cycle phase",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "GestationProgress",
                        ObservationTypeId = timeDaysType.Id,
                        MinimumValue = 0m,
                        MaximumValue = 70m,
                        Description = "Quantitative measurement of gestation time in days",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "CoatCondition",
                        ObservationTypeId = groomingType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 5m,
                        Description = "Semi-quantitative score of coat health",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "DentalHealth",
                        ObservationTypeId = groomingType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 5m,
                        Description = "Semi-quantitative score of dental condition",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "PainAssessment",
                        ObservationTypeId = vitalSignsType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 10m,
                        Description = "Semi-quantitative score of pain level",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        DefinitionName = "AmbientHumidity",
                        ObservationTypeId = environmentType.Id,
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
                var weighIn = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "WeighIn");
                var tempCheck = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "TempCheck");
                var behaviorNote = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "BehaviorNote");
                var medicationDose = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "MedicationDose");
                var litterSize = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "LitterSize");
                var heartRate = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "HeartRate");
                var respiratoryRate = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "RespiratoryRate");
                var hydrationStatus = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "HydrationStatus");
                var exerciseDuration = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "ExerciseDuration");
                var exerciseIntensity = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "ExerciseIntensity");
                var appetiteLevel = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "AppetiteLevel");
                var stoolQuality = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "StoolQuality");
                var dailyCaloricIntake = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "DailyCaloricIntake");
                var socializationBehavior = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "SocializationBehavior");
                var trainingProgress = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "TrainingProgress");
                var estrusCycleStage = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "EstrusCycleStage");
                var gestationProgress = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "GestationProgress");
                var coatCondition = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "CoatCondition");
                var dentalHealth = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "DentalHealth");
                var painAssessment = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "PainAssessment");
                var ambientHumidity = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "AmbientHumidity");
                var canineLifeStageFactor = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "CanineLifeStageFactor");
                var mtDNA = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "mtDNA");
                var yDNA = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "Y-DNA");
                var canineGeneticHealthConditionStatus = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "CanineGeneticHealthConditionStatus");
                var canineOfaHipGrade = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "CanineOfaHipGrade");

                var canineBiology = await context.ScientificDisciplines.FirstAsync(sd => sd.DisciplineName == "CanineBiology");
                var nutritionScience = await context.ScientificDisciplines.FirstAsync(sd => sd.DisciplineName == "NutritionScience");
                var veterinaryMedicine = await context.ScientificDisciplines.FirstAsync(sd => sd.DisciplineName == "VeterinaryMedicine");
                var ethology = await context.ScientificDisciplines.FirstAsync(sd => sd.DisciplineName == "Ethology");
                var pharmacology = await context.ScientificDisciplines.FirstAsync(sd => sd.DisciplineName == "Pharmacology");

                context.Set<Dictionary<string, object>>("ObservationDefinitionDiscipline").AddRange(
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", mtDNA.Id },
                        { "ScientificDisciplineId", canineBiology.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", yDNA.Id },
                        { "ScientificDisciplineId", canineBiology.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", canineGeneticHealthConditionStatus.Id },
                        { "ScientificDisciplineId", canineBiology.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", canineOfaHipGrade.Id },
                        { "ScientificDisciplineId", canineBiology.Id }
                    },
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
                        { "ObservationDefinitionId", canineLifeStageFactor.Id },
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
                var weighIn = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "WeighIn");
                var tempCheck = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "TempCheck");
                var medicationDose = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "MedicationDose");
                var litterSize = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "LitterSize");
                var heartRate = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "HeartRate");
                var respiratoryRate = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "RespiratoryRate");
                var hydrationStatus = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "HydrationStatus");
                var exerciseDuration = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "ExerciseDuration");
                var stoolQuality = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "StoolQuality");
                var dailyCaloricIntake = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "DailyCaloricIntake");
                var trainingProgress = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "TrainingProgress");
                var gestationProgress = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "GestationProgress");
                var coatCondition = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "CoatCondition");
                var dentalHealth = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "DentalHealth");
                var painAssessment = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "PainAssessment");
                var ambientHumidity = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "AmbientHumidity");

                var kilograms = await context.Units.FirstAsync(u => u.UnitName == "Kilograms");
                var pounds = await context.Units.FirstAsync(u => u.UnitName == "Pounds");
                var ounces = await context.Units.FirstAsync(u => u.UnitName == "Ounces");
                var celsius = await context.Units.FirstAsync(u => u.UnitName == "DegreesCelsius");
                var fahrenheit = await context.Units.FirstAsync(u => u.UnitName == "DegreesFahrenheit");
                var milliliters = await context.Units.FirstAsync(u => u.UnitName == "Milliliters");
                var milligrams = await context.Units.FirstAsync(u => u.UnitName == "Milligrams");
                var grams = await context.Units.FirstAsync(u => u.UnitName == "Grams");
                var count = await context.Units.FirstAsync(u => u.UnitName == "Count");
                var bpm = await context.Units.FirstAsync(u => u.UnitName == "BeatsPerMinute");
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
                var weighIn = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "WeighIn");
                var tempCheck = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "TempCheck");
                var medicationDose = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "MedicationDose");
                var litterSize = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "LitterSize");
                var heartRate = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "HeartRate");
                var respiratoryRate = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "RespiratoryRate");
                var hydrationStatus = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "HydrationStatus");
                var exerciseDuration = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "ExerciseDuration");
                var stoolQuality = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "StoolQuality");
                var dailyCaloricIntake = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "DailyCaloricIntake");
                var trainingProgress = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "TrainingProgress");
                var gestationProgress = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "GestationProgress");
                var coatCondition = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "CoatCondition");
                var dentalHealth = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "DentalHealth");
                var painAssessment = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "PainAssessment");
                var ambientHumidity = await context.ObservationDefinitions.FirstAsync(od => od.DefinitionName == "AmbientHumidity");

                var kilograms = await context.Units.FirstAsync(u => u.UnitName == "Kilograms");
                var pounds = await context.Units.FirstAsync(u => u.UnitName == "Pounds");
                var ounces = await context.Units.FirstAsync(u => u.UnitName == "Ounces");
                var celsius = await context.Units.FirstAsync(u => u.UnitName == "DegreesCelsius");
                var fahrenheit = await context.Units.FirstAsync(u => u.UnitName == "DegreesFahrenheit");
                var milliliters = await context.Units.FirstAsync(u => u.UnitName == "Milliliters");
                var milligrams = await context.Units.FirstAsync(u => u.UnitName == "Milligrams");
                var grams = await context.Units.FirstAsync(u => u.UnitName == "Grams");
                var count = await context.Units.FirstAsync(u => u.UnitName == "Count");
                var bpm = await context.Units.FirstAsync(u => u.UnitName == "BeatsPerMinute");
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
                    new MetaTag { TagName = "HealthCheck", Description = "General health or veterinary check-up", IsActive = true },
                    new MetaTag { TagName = "Behavior", Description = "Observations about behavior or temperament", IsActive = true },
                    // New Entries
                    new MetaTag { TagName = "VitalSigns", Description = "Related to physiological measurements", IsActive = true },
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