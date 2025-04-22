using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BiermanTech.CriticalDog.Helpers;

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
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // Seed Units
                logger.LogInformation("Seeding Units...");
                context.ChangeTracker.Clear();
                foreach (var unit in SeedData.Units)
                {
                    var existingUnit = await context.Units
                        .FirstOrDefaultAsync(u => u.Name == unit.Name);
                    if (existingUnit == null)
                    {
                        context.Units.Add(unit);
                        logger.LogDebug($"Adding Unit: {unit.Name}");
                    }
                    else
                    {
                        existingUnit.UnitSymbol = unit.UnitSymbol;
                        existingUnit.Description = unit.Description;
                        existingUnit.IsActive = unit.IsActive;
                        context.Units.Update(existingUnit);
                        logger.LogDebug($"Updating Unit: {unit.Name}");
                    }
                }
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();

                // Seed ObservationTypes
                logger.LogInformation("Seeding ObservationTypes...");
                context.ChangeTracker.Clear();
                foreach (var ot in SeedData.ObservationTypes)
                {
                    var existingOt = await context.ObservationTypes
                        .FirstOrDefaultAsync(o => o.Name == ot.Name);
                    if (existingOt == null)
                    {
                        context.ObservationTypes.Add(ot);
                        logger.LogDebug($"Adding ObservationType: {ot.Name}");
                    }
                    else
                    {
                        existingOt.Description = ot.Description;
                        existingOt.IsActive = ot.IsActive;
                        context.ObservationTypes.Update(existingOt);
                        logger.LogDebug($"Updating ObservationType: {ot.Name}");
                    }
                }
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();

                // Seed ScientificDisciplines
                logger.LogInformation("Seeding ScientificDisciplines...");
                context.ChangeTracker.Clear();
                foreach (var sd in SeedData.ScientificDisciplines)
                {
                    var existingSd = await context.ScientificDisciplines
                        .FirstOrDefaultAsync(s => s.Name == sd.Name);
                    if (existingSd == null)
                    {
                        context.ScientificDisciplines.Add(sd);
                        logger.LogDebug($"Adding ScientificDiscipline: {sd.Name}");
                    }
                    else
                    {
                        existingSd.Description = sd.Description;
                        existingSd.IsActive = sd.IsActive;
                        context.ScientificDisciplines.Update(existingSd);
                        logger.LogDebug($"Updating ScientificDiscipline: {sd.Name}");
                    }
                }
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();

                // Seed ObservationDefinitions
                logger.LogInformation("Seeding ObservationDefinitions...");
                context.ChangeTracker.Clear();
                var observationTypes = await context.ObservationTypes.ToDictionaryAsync(ot => ot.Name, ot => ot);
                foreach (var od in SeedData.ObservationDefinitions)
                {
                    var existingOd = await context.ObservationDefinitions
                        .FirstOrDefaultAsync(o => o.Name == od.Name);
                    if (existingOd != null)
                    {
                        logger.LogDebug($"ObservationDefinition {od.Name} already exists, skipping.");
                        continue;
                    }

                    if (!observationTypes.TryGetValue(od.ObservationTypeName, out var observationType))
                    {
                        logger.LogWarning($"Skipping ObservationDefinition {od.Name}: ObservationType {od.ObservationTypeName} not found.");
                        continue;
                    }

                    var newOd = new ObservationDefinition
                    {
                        Name = od.Name,
                        ObservationTypeId = observationType.Id,
                        Description = od.Description,
                        MinimumValue = od.MinimumValue,
                        MaximumValue = od.MaximumValue,
                        IsActive = od.IsActive,
                        IsSingular = od.IsSingular
                    };
                    context.ObservationDefinitions.Add(newOd);
                    logger.LogDebug($"Adding ObservationDefinition: {od.Name}");
                }
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();

                // Seed ObservationDefinitionDiscipline
                logger.LogInformation("Seeding ObservationDefinitionDiscipline...");
                context.ChangeTracker.Clear();
                var observationDefinitions = await context.ObservationDefinitions.ToDictionaryAsync(od => od.Name, od => od);
                var scientificDisciplines = await context.ScientificDisciplines.ToDictionaryAsync(sd => sd.Name, sd => sd);
                await SeedRelationshipsAsync(
                    context,
                    logger,
                    "ObservationDefinitionDiscipline",
                    SeedData.ObservationDefinitionDisciplines,
                    name => Task.FromResult(observationDefinitions.GetValueOrDefault(name)),
                    name => Task.FromResult(scientificDisciplines.GetValueOrDefault(name)),
                    "ObservationDefinitionId",
                    "ScientificDisciplineId");

                // Seed ObservationDefinitionUnit
                logger.LogInformation("Seeding ObservationDefinitionUnit...");
                context.ChangeTracker.Clear();
                var units = await context.Units.ToDictionaryAsync(u => u.Name, u => u);
                await SeedRelationshipsAsync(
                    context,
                    logger,
                    "ObservationDefinitionUnit",
                    SeedData.ObservationDefinitionUnits,
                    name => Task.FromResult(observationDefinitions.GetValueOrDefault(name)),
                    name => Task.FromResult(units.GetValueOrDefault(name)),
                    "ObservationDefinitionId",
                    "UnitId");

                // Seed MetricTypes
                logger.LogInformation("Seeding MetricTypes...");
                context.ChangeTracker.Clear();
                foreach (var (odName, unitName) in SeedData.ObservationDefinitionUnits)
                {
                    var metricTypeName = $"{odName} in {unitName.ToLower()}";
                    var existingMt = await context.MetricTypes
                        .FirstOrDefaultAsync(mt => mt.Name == metricTypeName);
                    if (existingMt != null)
                    {
                        logger.LogDebug($"MetricType {metricTypeName} already exists, skipping.");
                        continue;
                    }

                    if (!observationDefinitions.TryGetValue(odName, out var od) || !units.TryGetValue(unitName, out var unit))
                    {
                        logger.LogWarning($"Skipping MetricType for {odName} with {unitName}: Entity not found.");
                        continue;
                    }

                    var newMetricType = new MetricType
                    {
                        UnitId = unit.Id,
                        Name = metricTypeName,
                        IsActive = true
                    };
                    context.MetricTypes.Add(newMetricType);
                    logger.LogDebug($"Adding MetricType: {metricTypeName}");
                }
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();

                // Seed ObservationDefinitionMetricType
                logger.LogInformation("Seeding ObservationDefinitionMetricType...");
                context.ChangeTracker.Clear();
                var metricTypes = await context.MetricTypes.ToDictionaryAsync(mt => mt.Name, mt => mt);
                await SeedRelationshipsAsync(
                    context,
                    logger,
                    "ObservationDefinitionMetricType",
                    SeedData.ObservationDefinitionUnits.Select(odu => (odu.ObservationDefinitionName, $"{odu.ObservationDefinitionName} in {odu.UnitName.ToLower()}")),
                    name => Task.FromResult(observationDefinitions.GetValueOrDefault(name)),
                    name => Task.FromResult(metricTypes.GetValueOrDefault(name)),
                    "ObservationDefinitionId",
                    "MetricTypeId");

                // Seed MetaTags
                logger.LogInformation("Seeding MetaTags...");
                context.ChangeTracker.Clear();
                foreach (var mt in SeedData.MetaTags)
                {
                    // Apply the same slugification as ApplyMetaTagNameTransformation
                    var slugifiedName = StringHelper.Slugify(mt.Name);
                    // Query for the slugified name, as that's what will be saved
                    var existingMt = await context.MetaTags
                        .FirstOrDefaultAsync(m => m.Name == slugifiedName);
                    if (existingMt == null)
                    {
                        // Create a new MetaTag with the slugified name
                        var newMt = new MetaTag
                        {
                            Name = slugifiedName,
                            Description = mt.Description,
                            IsActive = mt.IsActive
                        };
                        context.MetaTags.Add(newMt);
                        logger.LogDebug($"Adding MetaTag: {newMt.Name} (original: {mt.Name})");
                    }
                    else
                    {
                        // Update the existing MetaTag
                        existingMt.Description = mt.Description;
                        existingMt.IsActive = mt.IsActive;
                        context.MetaTags.Update(existingMt);
                        logger.LogDebug($"Updating MetaTag: {existingMt.Name} (original: {mt.Name})");
                    }
                }
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();

                // Seed SubjectTypes
                logger.LogInformation("Seeding SubjectTypes...");
                context.ChangeTracker.Clear();
                foreach (var st in SeedData.SubjectTypes)
                {
                    var existingSt = await context.SubjectTypes
                        .FirstOrDefaultAsync(s => s.Name == st.Name);
                    if (existingSt == null)
                    {
                        context.SubjectTypes.Add(st);
                        logger.LogDebug($"Adding SubjectType: {st.Name}");
                    }
                    else
                    {
                        existingSt.Description = st.Description;
                        context.SubjectTypes.Update(existingSt);
                        logger.LogDebug($"Updating SubjectType: {st.Name}");
                    }
                }
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError(ex, "An error occurred while seeding data.");
                throw;
            }
        }

        private static async Task SeedRelationshipsAsync<T1, T2>(
            AppDbContext context,
            ILogger logger,
            string tableName,
            IEnumerable<(string Entity1Name, string Entity2Name)> relationships,
            Func<string, Task<T1>> getEntity1,
            Func<string, Task<T2>> getEntity2,
            string entity1Key,
            string entity2Key)
        {
            logger.LogInformation($"Seeding {tableName}...");
            context.ChangeTracker.Clear();

            var existingRelationships = await context.Set<Dictionary<string, object>>(tableName)
                .Select(e => new { Entity1Id = e[entity1Key], Entity2Id = e[entity2Key] })
                .ToListAsync();

            var entries = new List<Dictionary<string, object>>();
            foreach (var (name1, name2) in relationships)
            {
                var entity1 = await getEntity1(name1);
                var entity2 = await getEntity2(name2);

                if (entity1 == null || entity2 == null)
                {
                    logger.LogWarning($"Skipping relationship: {name1} or {name2} not found.");
                    continue;
                }

                var entity1Id = entity1.GetType().GetProperty("Id").GetValue(entity1);
                var entity2Id = entity2.GetType().GetProperty("Id").GetValue(entity2);

                if (existingRelationships.Any(r => r.Entity1Id.Equals(entity1Id) && r.Entity2Id.Equals(entity2Id)))
                {
                    logger.LogDebug($"Relationship between {name1} and {name2} already exists, skipping.");
                    continue;
                }

                entries.Add(new Dictionary<string, object>
                {
                    { entity1Key, entity1Id },
                    { entity2Key, entity2Id }
                });
                logger.LogDebug($"Adding relationship: {name1} to {name2} in {tableName}");
            }

            if (entries.Any())
            {
                context.Set<Dictionary<string, object>>(tableName).AddRange(entries);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
            }
            else
            {
                logger.LogInformation($"No new relationships to seed for {tableName}.");
            }
        }

        private static class SeedData
        {
            public static readonly List<Unit> Units = new()
            {
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
            };

            public static readonly List<ObservationType> ObservationTypes = new()
            {
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
            };

            public static readonly List<ScientificDiscipline> ScientificDisciplines = new()
            {
                new ScientificDiscipline { Name = "CanineBiology", Description = "Study of dog anatomy, physiology, and genetics", IsActive = true },
                new ScientificDiscipline { Name = "NutritionScience", Description = "Study of dietary needs for canine health", IsActive = true },
                new ScientificDiscipline { Name = "VeterinaryMedicine", Description = "Diagnosis and treatment of canine diseases", IsActive = true },
                new ScientificDiscipline { Name = "Ethology", Description = "Study of animal behavior", IsActive = true },
                new ScientificDiscipline { Name = "Pharmacology", Description = "Study of drug effects on dogs", IsActive = true }
            };

            public static readonly List<ObservationDefinitionData> ObservationDefinitions = new()
            {
                new ObservationDefinitionData
                {
                    Name = "mtDNA",
                    ObservationTypeName = "Qualitative",
                    Description = "Record of mitochondrial DNA haplotype or haplogroup for maternal lineage analysis",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "Y-DNA",
                    ObservationTypeName = "Qualitative",
                    Description = "Record of Y-chromosome DNA haplotype or haplogroup for paternal lineage analysis",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "CanineGeneticHealthConditionStatus",
                    ObservationTypeName = "GeneticHealthConditionStatus",
                    MinimumValue = 1m,
                    MaximumValue = 4m,
                    Description = "Covers the possible outcomes for a canine genetic health condition",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "CanineOfaHipGrade",
                    ObservationTypeName = "OfaHipsGrade",
                    MinimumValue = 1m,
                    MaximumValue = 7m,
                    Description = "OFA radiographic evaluation of canine hip joint structure",
                    IsActive = true,
                    IsSingular = true
                },
                new ObservationDefinitionData
                {
                    Name = "WeighIn",
                    ObservationTypeName = "Weight",
                    MinimumValue = 0m,
                    MaximumValue = 200m,
                    Description = "Quantitative measurement of subject weight",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "IdealWeight",
                    ObservationTypeName = "Weight",
                    MinimumValue = 0m,
                    MaximumValue = 200m,
                    Description = "Quantitative measurement of subject's ideal weight",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "TempCheck",
                    ObservationTypeName = "Temperature",
                    MinimumValue = 36m,
                    MaximumValue = 40m,
                    Description = "Quantitative measurement of body temperature",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "BehaviorNote",
                    ObservationTypeName = "Behavior",
                    Description = "Qualitative note on subject behavior",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "MedicationDose",
                    ObservationTypeName = "Medication",
                    MinimumValue = 0m,
                    MaximumValue = 1000m,
                    Description = "Quantitative record of medication administered",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "LitterSize",
                    ObservationTypeName = "Cohort",
                    MinimumValue = 0m,
                    MaximumValue = 24m,
                    Description = "Count of offspring",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "HeartRate",
                    ObservationTypeName = "VitalSigns",
                    MinimumValue = 40m,
                    MaximumValue = 180m,
                    Description = "Quantitative measurement of heart rate",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "RespiratoryRate",
                    ObservationTypeName = "VitalSigns",
                    MinimumValue = 10m,
                    MaximumValue = 60m,
                    Description = "Quantitative measurement of respiratory rate",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "HydrationStatus",
                    ObservationTypeName = "VitalSigns",
                    MinimumValue = 0m,
                    MaximumValue = 100m,
                    Description = "Quantitative estimate of hydration level",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "ExerciseDuration",
                    ObservationTypeName = "ExerciseDuration",
                    MinimumValue = 0m,
                    MaximumValue = 300m,
                    Description = "Quantitative measurement of exercise time",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "ExerciseIntensity",
                    ObservationTypeName = "ExerciseIntensity",
                    MinimumValue = 1m,
                    MaximumValue = 4m,
                    Description = "Qualitative assessment of exercise intensity (low, moderate, high)",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "AppetiteLevel",
                    ObservationTypeName = "Nutrition",
                    Description = "Qualitative observation of food intake",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "CanineLifeStageFactor",
                    ObservationTypeName = "LifeStageFactor",
                    MinimumValue = 1m,
                    MaximumValue = 10m,
                    Description = "Used in nutrition to determine energy requirements",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "StoolQuality",
                    ObservationTypeName = "StoolQualityScore",
                    MinimumValue = 1m,
                    MaximumValue = 7m,
                    Description = "Score of stool consistency (1–7 scale)",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "DailyCaloricIntake",
                    ObservationTypeName = "Nutrition",
                    MinimumValue = 0m,
                    MaximumValue = 5000m,
                    Description = "Quantitative measurement of daily energy intake",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "SocializationBehavior",
                    ObservationTypeName = "Behavior",
                    Description = "Qualitative note on socialization with dogs or humans",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "TrainingProgress",
                    ObservationTypeName = "Behavior",
                    MinimumValue = 1m,
                    MaximumValue = 5m,
                    Description = "Semi-quantitative score of training command mastery",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "EstrusCycleStage",
                    ObservationTypeName = "EstrusStage",
                    MinimumValue = 1m,
                    MaximumValue = 4m,
                    Description = "Qualitative observation of reproductive cycle phase",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "GestationProgress",
                    ObservationTypeName = "TemporalDays",
                    MinimumValue = 0m,
                    MaximumValue = 70m,
                    Description = "Quantitative measurement of gestation time in days",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "CoatCondition",
                    ObservationTypeName = "Grooming",
                    MinimumValue = 1m,
                    MaximumValue = 5m,
                    Description = "Semi-quantitative score of coat health",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "DentalHealth",
                    ObservationTypeName = "Grooming",
                    MinimumValue = 1m,
                    MaximumValue = 5m,
                    Description = "Semi-quantitative score of dental condition",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "PainAssessment",
                    ObservationTypeName = "VitalSigns",
                    MinimumValue = 1m,
                    MaximumValue = 10m,
                    Description = "Semi-quantitative score of pain level",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "AmbientHumidity",
                    ObservationTypeName = "Environment",
                    MinimumValue = 0m,
                    MaximumValue = 100m,
                    Description = "Quantitative measurement of environmental humidity",
                    IsActive = true
                }
            };

            public static readonly List<(string ObservationDefinitionName, string ScientificDisciplineName)> ObservationDefinitionDisciplines = new()
            {
                ("mtDNA", "CanineBiology"),
                ("Y-DNA", "CanineBiology"),
                ("CanineGeneticHealthConditionStatus", "CanineBiology"),
                ("CanineOfaHipGrade", "CanineBiology"),
                ("WeighIn", "CanineBiology"),
                ("WeighIn", "NutritionScience"),
                ("IdealWeight", "CanineBiology"),
                ("IdealWeight", "NutritionScience"),
                ("TempCheck", "VeterinaryMedicine"),
                ("BehaviorNote", "Ethology"),
                ("MedicationDose", "VeterinaryMedicine"),
                ("MedicationDose", "Pharmacology"),
                ("LitterSize", "CanineBiology"),
                ("HeartRate", "VeterinaryMedicine"),
                ("RespiratoryRate", "VeterinaryMedicine"),
                ("HydrationStatus", "VeterinaryMedicine"),
                ("ExerciseDuration", "CanineBiology"),
                ("ExerciseIntensity", "Ethology"),
                ("AppetiteLevel", "NutritionScience"),
                ("StoolQuality", "VeterinaryMedicine"),
                ("DailyCaloricIntake", "NutritionScience"),
                ("CanineLifeStageFactor", "NutritionScience"),
                ("SocializationBehavior", "Ethology"),
                ("TrainingProgress", "Ethology"),
                ("EstrusCycleStage", "CanineBiology"),
                ("GestationProgress", "CanineBiology"),
                ("CoatCondition", "VeterinaryMedicine"),
                ("DentalHealth", "VeterinaryMedicine"),
                ("PainAssessment", "VeterinaryMedicine"),
                ("AmbientHumidity", "CanineBiology")
            };

            public static readonly List<(string ObservationDefinitionName, string UnitName)> ObservationDefinitionUnits = new()
            {
                ("WeighIn", "Kilograms"),
                ("WeighIn", "Grams"),
                ("WeighIn", "Milligrams"),
                ("WeighIn", "Pounds"),
                ("WeighIn", "Ounces"),
                ("IdealWeight", "Kilograms"),
                ("IdealWeight", "Grams"),
                ("IdealWeight", "Milligrams"),
                ("IdealWeight", "Pounds"),
                ("IdealWeight", "Ounces"),
                ("TempCheck", "DegreesCelsius"),
                ("TempCheck", "DegreesFahrenheit"),
                ("MedicationDose", "Milliliters"),
                ("MedicationDose", "Milligrams"),
                ("MedicationDose", "Grams"),
                ("MedicationDose", "Count"),
                ("LitterSize", "Count"),
                ("HeartRate", "BeatsPerMinute"),
                ("RespiratoryRate", "BeatsPerMinute"),
                ("HydrationStatus", "Percentage"),
                ("ExerciseDuration", "Minutes"),
                ("StoolQuality", "Score"),
                ("DailyCaloricIntake", "Kilocalories"),
                ("TrainingProgress", "Score"),
                ("GestationProgress", "Days"),
                ("CoatCondition", "Score"),
                ("DentalHealth", "Score"),
                ("PainAssessment", "Score"),
                ("AmbientHumidity", "Percentage")
            };

            public static readonly List<MetaTag> MetaTags = new()
            {
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
            };

            public static readonly List<SubjectType> SubjectTypes = new()
            {
                new SubjectType { Name = "Dog", Description = "The dog (Canis familiaris or Canis lupus familiaris) is a domesticated descendant of the gray wolf." }
            };
        }

        private class ObservationDefinitionData
        {
            public string Name { get; set; }
            public string ObservationTypeName { get; set; }
            public string Description { get; set; }
            public decimal? MinimumValue { get; set; }
            public decimal? MaximumValue { get; set; }
            public bool IsActive { get; set; }
            public bool IsSingular { get; set; }
        }
    }
}