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
                    new Unit { Name = "Kilograms", UnitSymbol = "kg", Description = "Metric unit of mass", IsActive = true },
                    new Unit { Name = "Pounds", UnitSymbol = "lb", Description = "Imperial unit of mass", IsActive = true },
                    new Unit { Name = "Ounces", UnitSymbol = "oz", Description = "Imperial unit of mass", IsActive = true },
                    new Unit { Name = "Grams", UnitSymbol = "g", Description = "Metric unit of mass", IsActive = true },
                    new Unit { Name = "Milligrams", UnitSymbol = "mg", Description = "Metric unit of mass", IsActive = true },
                    new Unit { Name = "Milliliters", UnitSymbol = "mL", Description = "Metric unit of volume", IsActive = true },
                    new Unit { Name = "Liters", UnitSymbol = "L", Description = "Metric unit of volume", IsActive = true },
                    new Unit { Name = "Cups", UnitSymbol = "c", Description = "US customary unit of volume", IsActive = true },
                    new Unit { Name = "FluidOunces", UnitSymbol = "fl oz", Description = "US customary unit of volume", IsActive = true },
                    new Unit { Name = "Kilocalories", UnitSymbol = "kcal", Description = "Metric unit of energy", IsActive = true },
                    new Unit { Name = "Calories", UnitSymbol = "cal", Description = "Metric unit of energy", IsActive = true },
                    new Unit { Name = "Kilojoules", UnitSymbol = "kJ", Description = "Metric unit of energy", IsActive = true },
                    new Unit { Name = "DegreesCelsius", UnitSymbol = "°C", Description = "Metric unit of temperature", IsActive = true },
                    new Unit { Name = "DegreesFahrenheit", UnitSymbol = "°F", Description = "Imperial unit of temperature", IsActive = true },
                    new Unit { Name = "Centimeters", UnitSymbol = "cm", Description = "Metric unit of length", IsActive = true },
                    new Unit { Name = "Inches", UnitSymbol = "in", Description = "Imperial unit of length", IsActive = true },
                    new Unit { Name = "Minutes", UnitSymbol = "min", Description = "Unit of time", IsActive = true },
                    new Unit { Name = "Days", UnitSymbol = "d", Description = "Unit of time", IsActive = true },
                    new Unit { Name = "Hours", UnitSymbol = "h", Description = "Unit of time", IsActive = true },
                    new Unit { Name = "Weeks", UnitSymbol = "wk", Description = "Unit of time", IsActive = true },
                    new Unit { Name = "Years", UnitSymbol = "yr", Description = "Unit of time", IsActive = true },
                    new Unit { Name = "SquareMeters", UnitSymbol = "m²", Description = "Metric unit of area", IsActive = true },
                    new Unit { Name = "InternationalUnits", UnitSymbol = "IU", Description = "Unit for biological activity (e.g., vitamins)", IsActive = true },
                    new Unit { Name = "Percentage", UnitSymbol = "%", Description = "Unit of proportion or ratio", IsActive = true },
                    new Unit { Name = "Count", UnitSymbol = "ct", Description = "Denotes a quantity or number of items", IsActive = true },
                    new Unit { Name = "BeatsPerMinute", UnitSymbol = "bpm", Description = "Unit for heart rate or respiratory rate", IsActive = true },
                    new Unit { Name = "Score", UnitSymbol = "scr", Description = "Unit for semi-quantitative scales (e.g., 1–10)", IsActive = true }
                );
                await context.SaveChangesAsync();
            }

            // Seed ObservationTypes
            if (!await context.ObservationTypes.AnyAsync())
            {
                logger.LogInformation("Seeding ObservationTypes...");
                context.ObservationTypes.AddRange(
                    new ObservationType { Name = "Qualitative", Description = "Descriptive, often categorical and discrete", IsActive = true },
                    new ObservationType { Name = "Weight", Description = "Measurement of mass", IsActive = true },
                    new ObservationType { Name = "Cohort", Description = "Number of members in a cohort", IsActive = true },
                    new ObservationType { Name = "EstrusStage", Description = "Defines the stages of an estrus cycle", IsActive = true },
                    new ObservationType { Name = "Temperature", Description = "Measurement of body temperature", IsActive = true },
                    new ObservationType { Name = "TemporalDays", Description = "Measurement of time in days", IsActive = true },
                    new ObservationType { Name = "Behavior", Description = "Observation of behavioral traits", IsActive = true },
                    new ObservationType { Name = "Medication", Description = "Administration of medication", IsActive = true },
                    new ObservationType { Name = "GeneticHealthConditionStatus", Description = "Covers the possible outcomes for a canine genetic health condition", IsActive = true },
                    new ObservationType { Name = "OfaHipsGrade", Description = "OFA radiographic evaluation of canine hip joint structure", IsActive = true },
                    new ObservationType { Name = "VitalSigns", Description = "Measurement of physiological vital signs", IsActive = true },
                    new ObservationType { Name = "ExerciseDuration", Description = "Observation of physical activity", IsActive = true },
                    new ObservationType { Name = "ExerciseIntensity", Description = "Observation of physical activity", IsActive = true },
                    new ObservationType { Name = "Nutrition", Description = "Observation of dietary intake or digestion", IsActive = true },
                    new ObservationType { Name = "LifeStageFactor", Description = "Used in nutrition to determine energy requirements", IsActive = true },
                    new ObservationType { Name = "Grooming", Description = "Observation of coat, skin, or dental condition", IsActive = true },
                    new ObservationType { Name = "Environment", Description = "Observation of environmental conditions", IsActive = true },
                    new ObservationType { Name = "StoolQualityScore", Description = "Score of stool consistency (1–7 scale)", IsActive = true }
                );
                await context.SaveChangesAsync();
            }

            // Seed ScientificDisciplines
            if (!await context.ScientificDisciplines.AnyAsync())
            {
                logger.LogInformation("Seeding ScientificDisciplines...");
                context.ScientificDisciplines.AddRange(
                    new ScientificDiscipline { Name = "CanineBiology", Description = "Study of dog anatomy, physiology, and genetics", IsActive = true },
                    new ScientificDiscipline { Name = "NutritionScience", Description = "Study of dietary needs for canine health", IsActive = true },
                    new ScientificDiscipline { Name = "VeterinaryMedicine", Description = "Diagnosis and treatment of canine diseases", IsActive = true },
                    new ScientificDiscipline { Name = "Ethology", Description = "Study of animal behavior", IsActive = true },
                    new ScientificDiscipline { Name = "Pharmacology", Description = "Study of drug effects on dogs", IsActive = true }
                );
                await context.SaveChangesAsync();
            }

            // Seed ObservationDefinitions
            if (!await context.ObservationDefinitions.AnyAsync())
            {
                logger.LogInformation("Seeding ObservationDefinitions...");
                var qualitativeType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "Qualitative");
                var weightType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "Weight");
                var cohortType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "Cohort");
                var estrusStageType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "EstrusStage");
                var tempType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "Temperature");
                var timeDaysType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "TemporalDays");
                var behaviorType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "Behavior");
                var medicationType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "Medication");
                var vitalSignsType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "VitalSigns");
                var exerciseDurationType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "ExerciseDuration");
                var exerciseIntensityType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "ExerciseIntensity");
                var geneticConditionType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "GeneticHealthConditionStatus");
                var ofaHipsGradeType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "OfaHipsGrade");
                var nutritionType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "Nutrition");
                var lifeStageFactorType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "LifeStageFactor");
                var groomingType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "Grooming");
                var environmentType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "Environment");
                var stoolQualityType = await context.ObservationTypes.FirstAsync(ot => ot.Name == "StoolQualityScore");

                context.ObservationDefinitions.AddRange(
                    new ObservationDefinition
                    {
                        Name = "mtDNA",
                        ObservationTypeId = qualitativeType.Id,
                        Description = "Record of mitochondrial DNA haplotype or haplogroup for maternal lineage analysis",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "Y-DNA",
                        ObservationTypeId = qualitativeType.Id,
                        Description = "Record of Y-chromosome DNA haplotype or haplogroup for paternal lineage analysis",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "CanineGeneticHealthConditionStatus",
                        ObservationTypeId = geneticConditionType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 4m,
                        Description = "Covers the possible outcomes for a canine genetic health condition",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "CanineOfaHipGrade",
                        ObservationTypeId = ofaHipsGradeType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 7m,
                        Description = "OFA radiographic evaluation of canine hip joint structure",
                        IsActive = true,
                        IsSingular = true
                    },
                    new ObservationDefinition
                    {
                        Name = "WeighIn",
                        ObservationTypeId = weightType.Id,
                        MinimumValue = 0m,
                        MaximumValue = 200m,
                        Description = "Quantitative measurement of subject weight",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "TempCheck",
                        ObservationTypeId = tempType.Id,
                        MinimumValue = 36m,
                        MaximumValue = 40m,
                        Description = "Quantitative measurement of body temperature",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "BehaviorNote",
                        ObservationTypeId = behaviorType.Id,
                        MinimumValue = null,
                        MaximumValue = null,
                        Description = "Qualitative note on subject behavior",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "MedicationDose",
                        ObservationTypeId = medicationType.Id,
                        MinimumValue = 0m,
                        MaximumValue = 1000m,
                        Description = "Quantitative record of medication administered",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "LitterSize",
                        ObservationTypeId = cohortType.Id,
                        MinimumValue = 0m,
                        MaximumValue = 24m,
                        Description = "Count of offspring",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "HeartRate",
                        ObservationTypeId = vitalSignsType.Id,
                        MinimumValue = 40m,
                        MaximumValue = 180m,
                        Description = "Quantitative measurement of heart rate",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "RespiratoryRate",
                        ObservationTypeId = vitalSignsType.Id,
                        MinimumValue = 10m,
                        MaximumValue = 60m,
                        Description = "Quantitative measurement of respiratory rate",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "HydrationStatus",
                        ObservationTypeId = vitalSignsType.Id,
                        MinimumValue = 0m,
                        MaximumValue = 100m,
                        Description = "Quantitative estimate of hydration level",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "ExerciseDuration",
                        ObservationTypeId = exerciseDurationType.Id,
                        MinimumValue = 0m,
                        MaximumValue = 300m,
                        Description = "Quantitative measurement of exercise time",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "ExerciseIntensity",
                        ObservationTypeId = exerciseIntensityType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 4m,
                        Description = "Qualitative assessment of exercise intensity (low, moderate, high)",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "AppetiteLevel",
                        ObservationTypeId = nutritionType.Id,
                        MinimumValue = null,
                        MaximumValue = null,
                        Description = "Qualitative observation of food intake",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "CanineLifeStageFactor",
                        ObservationTypeId = lifeStageFactorType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 10m,
                        Description = "Used in nutrition to determine energy requirements",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "StoolQuality",
                        ObservationTypeId = stoolQualityType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 7m,
                        Description = "Score of stool consistency (1–7 scale)",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "DailyCaloricIntake",
                        ObservationTypeId = nutritionType.Id,
                        MinimumValue = 0m,
                        MaximumValue = 5000m,
                        Description = "Quantitative measurement of daily energy intake",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "SocializationBehavior",
                        ObservationTypeId = behaviorType.Id,
                        MinimumValue = null,
                        MaximumValue = null,
                        Description = "Qualitative note on socialization with dogs or humans",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "TrainingProgress",
                        ObservationTypeId = behaviorType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 5m,
                        Description = "Semi-quantitative score of training command mastery",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "EstrusCycleStage",
                        ObservationTypeId = estrusStageType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 4m,
                        Description = "Qualitative observation of reproductive cycle phase",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "GestationProgress",
                        ObservationTypeId = timeDaysType.Id,
                        MinimumValue = 0m,
                        MaximumValue = 70m,
                        Description = "Quantitative measurement of gestation time in days",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "CoatCondition",
                        ObservationTypeId = groomingType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 5m,
                        Description = "Semi-quantitative score of coat health",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "DentalHealth",
                        ObservationTypeId = groomingType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 5m,
                        Description = "Semi-quantitative score of dental condition",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "PainAssessment",
                        ObservationTypeId = vitalSignsType.Id,
                        MinimumValue = 1m,
                        MaximumValue = 10m,
                        Description = "Semi-quantitative score of pain level",
                        IsActive = true
                    },
                    new ObservationDefinition
                    {
                        Name = "AmbientHumidity",
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
                var weighIn = await context.ObservationDefinitions.FirstAsync(od => od.Name == "WeighIn");
                var tempCheck = await context.ObservationDefinitions.FirstAsync(od => od.Name == "TempCheck");
                var behaviorNote = await context.ObservationDefinitions.FirstAsync(od => od.Name == "BehaviorNote");
                var medicationDose = await context.ObservationDefinitions.FirstAsync(od => od.Name == "MedicationDose");
                var litterSize = await context.ObservationDefinitions.FirstAsync(od => od.Name == "LitterSize");
                var heartRate = await context.ObservationDefinitions.FirstAsync(od => od.Name == "HeartRate");
                var respiratoryRate = await context.ObservationDefinitions.FirstAsync(od => od.Name == "RespiratoryRate");
                var hydrationStatus = await context.ObservationDefinitions.FirstAsync(od => od.Name == "HydrationStatus");
                var exerciseDuration = await context.ObservationDefinitions.FirstAsync(od => od.Name == "ExerciseDuration");
                var exerciseIntensity = await context.ObservationDefinitions.FirstAsync(od => od.Name == "ExerciseIntensity");
                var appetiteLevel = await context.ObservationDefinitions.FirstAsync(od => od.Name == "AppetiteLevel");
                var stoolQuality = await context.ObservationDefinitions.FirstAsync(od => od.Name == "StoolQuality");
                var dailyCaloricIntake = await context.ObservationDefinitions.FirstAsync(od => od.Name == "DailyCaloricIntake");
                var socializationBehavior = await context.ObservationDefinitions.FirstAsync(od => od.Name == "SocializationBehavior");
                var trainingProgress = await context.ObservationDefinitions.FirstAsync(od => od.Name == "TrainingProgress");
                var estrusCycleStage = await context.ObservationDefinitions.FirstAsync(od => od.Name == "EstrusCycleStage");
                var gestationProgress = await context.ObservationDefinitions.FirstAsync(od => od.Name == "GestationProgress");
                var coatCondition = await context.ObservationDefinitions.FirstAsync(od => od.Name == "CoatCondition");
                var dentalHealth = await context.ObservationDefinitions.FirstAsync(od => od.Name == "DentalHealth");
                var painAssessment = await context.ObservationDefinitions.FirstAsync(od => od.Name == "PainAssessment");
                var ambientHumidity = await context.ObservationDefinitions.FirstAsync(od => od.Name == "AmbientHumidity");
                var canineLifeStageFactor = await context.ObservationDefinitions.FirstAsync(od => od.Name == "CanineLifeStageFactor");
                var mtDNA = await context.ObservationDefinitions.FirstAsync(od => od.Name == "mtDNA");
                var yDNA = await context.ObservationDefinitions.FirstAsync(od => od.Name == "Y-DNA");
                var canineGeneticHealthConditionStatus = await context.ObservationDefinitions.FirstAsync(od => od.Name == "CanineGeneticHealthConditionStatus");
                var canineOfaHipGrade = await context.ObservationDefinitions.FirstAsync(od => od.Name == "CanineOfaHipGrade");

                var canineBiology = await context.ScientificDisciplines.FirstAsync(sd => sd.Name == "CanineBiology");
                var nutritionScience = await context.ScientificDisciplines.FirstAsync(sd => sd.Name == "NutritionScience");
                var veterinaryMedicine = await context.ScientificDisciplines.FirstAsync(sd => sd.Name == "VeterinaryMedicine");
                var ethology = await context.ScientificDisciplines.FirstAsync(sd => sd.Name == "Ethology");
                var pharmacology = await context.ScientificDisciplines.FirstAsync(sd => sd.Name == "Pharmacology");

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
                var weighIn = await context.ObservationDefinitions.FirstAsync(od => od.Name == "WeighIn");
                var tempCheck = await context.ObservationDefinitions.FirstAsync(od => od.Name == "TempCheck");
                var medicationDose = await context.ObservationDefinitions.FirstAsync(od => od.Name == "MedicationDose");
                var litterSize = await context.ObservationDefinitions.FirstAsync(od => od.Name == "LitterSize");
                var heartRate = await context.ObservationDefinitions.FirstAsync(od => od.Name == "HeartRate");
                var respiratoryRate = await context.ObservationDefinitions.FirstAsync(od => od.Name == "RespiratoryRate");
                var hydrationStatus = await context.ObservationDefinitions.FirstAsync(od => od.Name == "HydrationStatus");
                var exerciseDuration = await context.ObservationDefinitions.FirstAsync(od => od.Name == "ExerciseDuration");
                var stoolQuality = await context.ObservationDefinitions.FirstAsync(od => od.Name == "StoolQuality");
                var dailyCaloricIntake = await context.ObservationDefinitions.FirstAsync(od => od.Name == "DailyCaloricIntake");
                var trainingProgress = await context.ObservationDefinitions.FirstAsync(od => od.Name == "TrainingProgress");
                var gestationProgress = await context.ObservationDefinitions.FirstAsync(od => od.Name == "GestationProgress");
                var coatCondition = await context.ObservationDefinitions.FirstAsync(od => od.Name == "CoatCondition");
                var dentalHealth = await context.ObservationDefinitions.FirstAsync(od => od.Name == "DentalHealth");
                var painAssessment = await context.ObservationDefinitions.FirstAsync(od => od.Name == "PainAssessment");
                var ambientHumidity = await context.ObservationDefinitions.FirstAsync(od => od.Name == "AmbientHumidity");

                var kilograms = await context.Units.FirstAsync(u => u.Name == "Kilograms");
                var pounds = await context.Units.FirstAsync(u => u.Name == "Pounds");
                var ounces = await context.Units.FirstAsync(u => u.Name == "Ounces");
                var celsius = await context.Units.FirstAsync(u => u.Name == "DegreesCelsius");
                var fahrenheit = await context.Units.FirstAsync(u => u.Name == "DegreesFahrenheit");
                var milliliters = await context.Units.FirstAsync(u => u.Name == "Milliliters");
                var milligrams = await context.Units.FirstAsync(u => u.Name == "Milligrams");
                var grams = await context.Units.FirstAsync(u => u.Name == "Grams");
                var count = await context.Units.FirstAsync(u => u.Name == "Count");
                var bpm = await context.Units.FirstAsync(u => u.Name == "BeatsPerMinute");
                var percentage = await context.Units.FirstAsync(u => u.Name == "Percentage");
                var minutes = await context.Units.FirstAsync(u => u.Name == "Minutes");
                var score = await context.Units.FirstAsync(u => u.Name == "Score");
                var kilocalories = await context.Units.FirstAsync(u => u.Name == "Kilocalories");
                var days = await context.Units.FirstAsync(u => u.Name == "Days");

                context.Set<Dictionary<string, object>>("ObservationDefinitionUnit").AddRange(
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
                var kilograms = await context.Units.FirstAsync(u => u.Name == "Kilograms");
                var pounds = await context.Units.FirstAsync(u => u.Name == "Pounds");
                var ounces = await context.Units.FirstAsync(u => u.Name == "Ounces");
                var celsius = await context.Units.FirstAsync(u => u.Name == "DegreesCelsius");
                var fahrenheit = await context.Units.FirstAsync(u => u.Name == "DegreesFahrenheit");
                var milliliters = await context.Units.FirstAsync(u => u.Name == "Milliliters");
                var milligrams = await context.Units.FirstAsync(u => u.Name == "Milligrams");
                var grams = await context.Units.FirstAsync(u => u.Name == "Grams");
                var count = await context.Units.FirstAsync(u => u.Name == "Count");
                var bpm = await context.Units.FirstAsync(u => u.Name == "BeatsPerMinute");
                var percentage = await context.Units.FirstAsync(u => u.Name == "Percentage");
                var minutes = await context.Units.FirstAsync(u => u.Name == "Minutes");
                var score = await context.Units.FirstAsync(u => u.Name == "Score");
                var kilocalories = await context.Units.FirstAsync(u => u.Name == "Kilocalories");
                var days = await context.Units.FirstAsync(u => u.Name == "Days");

                context.MetricTypes.AddRange(
                    new MetricType { UnitId = kilograms.Id, Name = "Weight in kilograms", IsActive = true },
                    new MetricType { UnitId = milligrams.Id, Name = "Weight in milligrams", IsActive = true },
                    new MetricType { UnitId = grams.Id, Name = "Weight in grams", IsActive = true },
                    new MetricType { UnitId = pounds.Id, Name = "Weight in pounds", IsActive = true },
                    new MetricType { UnitId = ounces.Id, Name = "Weight in ounces", IsActive = true },
                    new MetricType { UnitId = celsius.Id, Name = "Temperature in Celsius", IsActive = true },
                    new MetricType { UnitId = fahrenheit.Id, Name = "Temperature in Fahrenheit", IsActive = true },
                    new MetricType { UnitId = milliliters.Id, Name = "Medication dose in milliliters", IsActive = true },
                    new MetricType { UnitId = milligrams.Id, Name = "Medication dose in milligrams", IsActive = true },
                    new MetricType { UnitId = grams.Id, Name = "Medication dose in grams", IsActive = true },
                    new MetricType { UnitId = count.Id, Name = "Medication dose count", IsActive = true },
                    new MetricType { UnitId = count.Id, Name = "Count of offspring", IsActive = true },
                    new MetricType { UnitId = bpm.Id, Name = "Heart rate in beats per minute", IsActive = true },
                    new MetricType { UnitId = bpm.Id, Name = "Respiratory rate in breaths per minute", IsActive = true },
                    new MetricType { UnitId = percentage.Id, Name = "Hydration level in percentage", IsActive = true },
                    new MetricType { UnitId = minutes.Id, Name = "Exercise duration in minutes", IsActive = true },
                    new MetricType { UnitId = score.Id, Name = "Stool quality score (1–7)", IsActive = true },
                    new MetricType { UnitId = kilocalories.Id, Name = "Daily caloric intake in kilocalories", IsActive = true },
                    new MetricType { UnitId = score.Id, Name = "Training progress score (1–5)", IsActive = true },
                    new MetricType { UnitId = days.Id, Name = "Gestation progress in days", IsActive = true },
                    new MetricType { UnitId = score.Id, Name = "Coat condition score (1–5)", IsActive = true },
                    new MetricType { UnitId = score.Id, Name = "Dental health score (1–5)", IsActive = true },
                    new MetricType { UnitId = score.Id, Name = "Pain assessment score (1–10)", IsActive = true },
                    new MetricType { UnitId = percentage.Id, Name = "Ambient humidity in percentage", IsActive = true }
                );
                await context.SaveChangesAsync();
            }

            // Seed ObservationDefinitionMetricType
            if (!await context.Set<Dictionary<string, object>>("ObservationDefinitionMetricType").AnyAsync())
            {
                logger.LogInformation("Seeding ObservationDefinitionMetricType...");
                var weighIn = await context.ObservationDefinitions.FirstAsync(od => od.Name == "WeighIn");
                var tempCheck = await context.ObservationDefinitions.FirstAsync(od => od.Name == "TempCheck");
                var medicationDose = await context.ObservationDefinitions.FirstAsync(od => od.Name == "MedicationDose");
                var litterSize = await context.ObservationDefinitions.FirstAsync(od => od.Name == "LitterSize");
                var heartRate = await context.ObservationDefinitions.FirstAsync(od => od.Name == "HeartRate");
                var respiratoryRate = await context.ObservationDefinitions.FirstAsync(od => od.Name == "RespiratoryRate");
                var hydrationStatus = await context.ObservationDefinitions.FirstAsync(od => od.Name == "HydrationStatus");
                var exerciseDuration = await context.ObservationDefinitions.FirstAsync(od => od.Name == "ExerciseDuration");
                var stoolQuality = await context.ObservationDefinitions.FirstAsync(od => od.Name == "StoolQuality");
                var dailyCaloricIntake = await context.ObservationDefinitions.FirstAsync(od => od.Name == "DailyCaloricIntake");
                var trainingProgress = await context.ObservationDefinitions.FirstAsync(od => od.Name == "TrainingProgress");
                var gestationProgress = await context.ObservationDefinitions.FirstAsync(od => od.Name == "GestationProgress");
                var coatCondition = await context.ObservationDefinitions.FirstAsync(od => od.Name == "CoatCondition");
                var dentalHealth = await context.ObservationDefinitions.FirstAsync(od => od.Name == "DentalHealth");
                var painAssessment = await context.ObservationDefinitions.FirstAsync(od => od.Name == "PainAssessment");
                var ambientHumidity = await context.ObservationDefinitions.FirstAsync(od => od.Name == "AmbientHumidity");

                var kilograms = await context.MetricTypes.FirstAsync(mt => mt.Name == "Weight in kilograms");
                var milligrams = await context.MetricTypes.FirstAsync(mt => mt.Name == "Weight in milligrams");
                var grams = await context.MetricTypes.FirstAsync(mt => mt.Name == "Weight in grams");
                var pounds = await context.MetricTypes.FirstAsync(mt => mt.Name == "Weight in pounds");
                var ounces = await context.MetricTypes.FirstAsync(mt => mt.Name == "Weight in ounces");
                var celsius = await context.MetricTypes.FirstAsync(mt => mt.Name == "Temperature in Celsius");
                var fahrenheit = await context.MetricTypes.FirstAsync(mt => mt.Name == "Temperature in Fahrenheit");
                var milliliters = await context.MetricTypes.FirstAsync(mt => mt.Name == "Medication dose in milliliters");
                var milligramsMed = await context.MetricTypes.FirstAsync(mt => mt.Name == "Medication dose in milligrams");
                var gramsMed = await context.MetricTypes.FirstAsync(mt => mt.Name == "Medication dose in grams");
                var countMed = await context.MetricTypes.FirstAsync(mt => mt.Name == "Medication dose count");
                var countOffspring = await context.MetricTypes.FirstAsync(mt => mt.Name == "Count of offspring");
                var bpmHeart = await context.MetricTypes.FirstAsync(mt => mt.Name == "Heart rate in beats per minute");
                var bpmResp = await context.MetricTypes.FirstAsync(mt => mt.Name == "Respiratory rate in breaths per minute");
                var percentageHydration = await context.MetricTypes.FirstAsync(mt => mt.Name == "Hydration level in percentage");
                var minutesExercise = await context.MetricTypes.FirstAsync(mt => mt.Name == "Exercise duration in minutes");
                var scoreStool = await context.MetricTypes.FirstAsync(mt => mt.Name == "Stool quality score (1–7)");
                var kilocaloriesIntake = await context.MetricTypes.FirstAsync(mt => mt.Name == "Daily caloric intake in kilocalories");
                var scoreTraining = await context.MetricTypes.FirstAsync(mt => mt.Name == "Training progress score (1–5)");
                var daysGestation = await context.MetricTypes.FirstAsync(mt => mt.Name == "Gestation progress in days");
                var scoreCoat = await context.MetricTypes.FirstAsync(mt => mt.Name == "Coat condition score (1–5)");
                var scoreDental = await context.MetricTypes.FirstAsync(mt => mt.Name == "Dental health score (1–5)");
                var scorePain = await context.MetricTypes.FirstAsync(mt => mt.Name == "Pain assessment score (1–10)");
                var percentageHumidity = await context.MetricTypes.FirstAsync(mt => mt.Name == "Ambient humidity in percentage");

                context.Set<Dictionary<string, object>>("ObservationDefinitionMetricType").AddRange(
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", weighIn.Id },
                        { "MetricTypeId", kilograms.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", weighIn.Id },
                        { "MetricTypeId", milligrams.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", weighIn.Id },
                        { "MetricTypeId", grams.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", weighIn.Id },
                        { "MetricTypeId", pounds.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", weighIn.Id },
                        { "MetricTypeId", ounces.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", tempCheck.Id },
                        { "MetricTypeId", celsius.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", tempCheck.Id },
                        { "MetricTypeId", fahrenheit.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", medicationDose.Id },
                        { "MetricTypeId", milliliters.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", medicationDose.Id },
                        { "MetricTypeId", milligramsMed.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", medicationDose.Id },
                        { "MetricTypeId", gramsMed.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", medicationDose.Id },
                        { "MetricTypeId", countMed.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", litterSize.Id },
                        { "MetricTypeId", countOffspring.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", heartRate.Id },
                        { "MetricTypeId", bpmHeart.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", respiratoryRate.Id },
                        { "MetricTypeId", bpmResp.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", hydrationStatus.Id },
                        { "MetricTypeId", percentageHydration.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", exerciseDuration.Id },
                        { "MetricTypeId", minutesExercise.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", stoolQuality.Id },
                        { "MetricTypeId", scoreStool.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", dailyCaloricIntake.Id },
                        { "MetricTypeId", kilocaloriesIntake.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", trainingProgress.Id },
                        { "MetricTypeId", scoreTraining.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", gestationProgress.Id },
                        { "MetricTypeId", daysGestation.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", coatCondition.Id },
                        { "MetricTypeId", scoreCoat.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", dentalHealth.Id },
                        { "MetricTypeId", scoreDental.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", painAssessment.Id },
                        { "MetricTypeId", scorePain.Id }
                    },
                    new Dictionary<string, object>
                    {
                        { "ObservationDefinitionId", ambientHumidity.Id },
                        { "MetricTypeId", percentageHumidity.Id }
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed MetaTags
            if (!await context.MetaTags.AnyAsync())
            {
                logger.LogInformation("Seeding MetaTags...");
                context.MetaTags.AddRange(
                    new MetaTag { Name = "Feeding", Description = "Related to food or feeding observations", IsActive = true },
                    new MetaTag { Name = "Medication", Description = "Indicates a medication-related record", IsActive = true },
                    new MetaTag { Name = "Exercise", Description = "Pertains to physical activity or exercise", IsActive = true },
                    new MetaTag { Name = "Grooming", Description = "Related to grooming or hygiene", IsActive = true },
                    new MetaTag { Name = "HealthCheck", Description = "General health or veterinary check-up", IsActive = true },
                    new MetaTag { Name = "Behavior", Description = "Observations about behavior or temperament", IsActive = true },
                    new MetaTag { Name = "VitalSigns", Description = "Related to physiological measurements", IsActive = true },
                    new MetaTag { Name = "Reproduction", Description = "Related to reproductive observations", IsActive = true },
                    new MetaTag { Name = "Training", Description = "Related to training or obedience observations", IsActive = true },
                    new MetaTag { Name = "Environment", Description = "Related to environmental conditions", IsActive = true }
                );
                await context.SaveChangesAsync();
            }

            // Seed SubjectTypes
            if (!await context.SubjectTypes.AnyAsync())
            {
                logger.LogInformation("Seeding SubjectTypes...");
                context.SubjectTypes.AddRange(
                    new SubjectType { Name = "Dog", Description = "The dog (Canis familiaris or Canis lupus familiaris) is a domesticated descendant of the gray wolf." }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}