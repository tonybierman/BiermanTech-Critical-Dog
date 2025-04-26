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
                    // Query for the slugified name and UserId = null (system-scoped)
                    var existingMt = await context.MetaTags
                        .FirstOrDefaultAsync(m => m.Name == slugifiedName && m.UserId == null);
                    if (existingMt == null)
                    {
                        // Create a new system-scoped MetaTag with UserId = null
                        var newMt = new MetaTag
                        {
                            Name = slugifiedName,
                            Description = mt.Description,
                            IsActive = mt.IsActive,
                            UserId = null // System-scoped
                        };
                        context.MetaTags.Add(newMt);
                        logger.LogDebug($"Adding system-scoped MetaTag: {newMt.Name} (original: {mt.Name})");
                    }
                    else
                    {
                        // Update the existing system-scoped MetaTag
                        existingMt.Description = mt.Description;
                        existingMt.IsActive = mt.IsActive;
                        context.MetaTags.Update(existingMt);
                        logger.LogDebug($"Updating system-scoped MetaTag: {existingMt.Name} (original: {mt.Name})");
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
                        existingSt.ScientificName = st.ScientificName;
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
                new ScientificDiscipline { Name = "Biology", Description = "Study of anatomy, physiology, and genetics", IsActive = true },
                new ScientificDiscipline { Name = "NutritionScience", Description = "Study of dietary needs for health", IsActive = true },
                new ScientificDiscipline { Name = "VeterinaryMedicine", Description = "Diagnosis and treatment of diseases", IsActive = true },
                new ScientificDiscipline { Name = "Ethology", Description = "Study of natural behavior", IsActive = true }
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
                    Name = "GeneticHealthCondition",
                    ObservationTypeName = "GeneticHealthConditionStatus",
                    MinimumValue = 1m,
                    MaximumValue = 4m,
                    Description = "Covers the possible outcomes for a canine genetic health condition",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "HipsScoreOFA",
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
                    Name = "AntibioticDose",
                    ObservationTypeName = "Medication",
                    MinimumValue = 0m,
                    MaximumValue = 1000m,
                    Description = "Quantitative record of antibiotic medication administered",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "AntihelminticDose",
                    ObservationTypeName = "Medication",
                    MinimumValue = 0m,
                    MaximumValue = 1000m,
                    Description = "Quantitative record of antihelmintic (dewormer) medication administered",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "FleaTickPreventativeDose",
                    ObservationTypeName = "Medication",
                    MinimumValue = 0m,
                    MaximumValue = 1000m,
                    Description = "Quantitative record of flea and tick preventative medication administered",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "HeartwormPreventativeDose",
                    ObservationTypeName = "Medication",
                    MinimumValue = 0m,
                    MaximumValue = 1000m,
                    Description = "Quantitative record of heartworm preventative medication administered",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "NSAIDDose",
                    ObservationTypeName = "Medication",
                    MinimumValue = 0m,
                    MaximumValue = 1000m,
                    Description = "Quantitative record of non-steroidal anti-inflammatory drug (NSAID) administered",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "SteroidDose",
                    ObservationTypeName = "Medication",
                    MinimumValue = 0m,
                    MaximumValue = 1000m,
                    Description = "Quantitative record of steroid medication administered",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "AntihistamineDose",
                    ObservationTypeName = "Medication",
                    MinimumValue = 0m,
                    MaximumValue = 1000m,
                    Description = "Quantitative record of antihistamine medication administered",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "AntifungalDose",
                    ObservationTypeName = "Medication",
                    MinimumValue = 0m,
                    MaximumValue = 1000m,
                    Description = "Quantitative record of antifungal medication administered",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "AntiepilepticDose",
                    ObservationTypeName = "Medication",
                    MinimumValue = 0m,
                    MaximumValue = 1000m,
                    Description = "Quantitative record of antiepileptic medication administered",
                    IsActive = true
                },
                new ObservationDefinitionData
                {
                    Name = "AnxiolyticDose",
                    ObservationTypeName = "Medication",
                    MinimumValue = 0m,
                    MaximumValue = 1000m,
                    Description = "Quantitative record of anxiolytic or behavior-modifying medication administered",
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
                    Name = "LifeStage",
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
                    Name = "Socialization",
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
                ("mtDNA", "Biology"),
                ("Y-DNA", "Biology"),
                ("GeneticHealthCondition", "Biology"),
                ("HipsScoreOFA", "Biology"),
                ("WeighIn", "NutritionScience"),
                ("IdealWeight", "NutritionScience"),
                ("TempCheck", "VeterinaryMedicine"),
                ("BehaviorNote", "Ethology"),
                ("MedicationDose", "VeterinaryMedicine"),
                ("AntibioticDose", "VeterinaryMedicine"),
                ("AntihelminticDose", "VeterinaryMedicine"),
                ("FleaTickPreventativeDose", "VeterinaryMedicine"),
                ("HeartwormPreventativeDose", "VeterinaryMedicine"),
                ("NSAIDDose", "VeterinaryMedicine"),
                ("SteroidDose", "VeterinaryMedicine"),
                ("AntihistamineDose", "VeterinaryMedicine"),
                ("AntifungalDose", "VeterinaryMedicine"),
                ("AntiepilepticDose", "VeterinaryMedicine"),
                ("AnxiolyticDose", "VeterinaryMedicine"),
                ("LitterSize", "Biology"),
                ("HeartRate", "VeterinaryMedicine"),
                ("RespiratoryRate", "VeterinaryMedicine"),
                ("HydrationStatus", "VeterinaryMedicine"),
                ("ExerciseDuration", "Biology"),
                ("ExerciseIntensity", "Ethology"),
                ("AppetiteLevel", "NutritionScience"),
                ("StoolQuality", "VeterinaryMedicine"),
                ("DailyCaloricIntake", "NutritionScience"),
                ("LifeStage", "NutritionScience"),
                ("Socialization", "Ethology"),
                ("TrainingProgress", "Ethology"),
                ("EstrusCycleStage", "Biology"),
                ("GestationProgress", "Biology"),
                ("CoatCondition", "VeterinaryMedicine"),
                ("DentalHealth", "VeterinaryMedicine"),
                ("PainAssessment", "VeterinaryMedicine"),
                ("AmbientHumidity", "Biology")
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
                ("AntibioticDose", "Milliliters"),
                ("AntibioticDose", "Milligrams"),
                ("AntibioticDose", "Grams"),
                ("AntibioticDose", "Count"),
                ("AntihelminticDose", "Milliliters"),
                ("AntihelminticDose", "Milligrams"),
                ("AntihelminticDose", "Grams"),
                ("AntihelminticDose", "Count"),
                ("FleaTickPreventativeDose", "Milliliters"),
                ("FleaTickPreventativeDose", "Milligrams"),
                ("FleaTickPreventativeDose", "Grams"),
                ("FleaTickPreventativeDose", "Count"),
                ("HeartwormPreventativeDose", "Milliliters"),
                ("HeartwormPreventativeDose", "Milligrams"),
                ("HeartwormPreventativeDose", "Grams"),
                ("HeartwormPreventativeDose", "Count"),
                ("NSAIDDose", "Milliliters"),
                ("NSAIDDose", "Milligrams"),
                ("NSAIDDose", "Grams"),
                ("NSAIDDose", "Count"),
                ("SteroidDose", "Milliliters"),
                ("SteroidDose", "Milligrams"),
                ("SteroidDose", "Grams"),
                ("SteroidDose", "Count"),
                ("AntihistamineDose", "Milliliters"),
                ("AntihistamineDose", "Milligrams"),
                ("AntihistamineDose", "Grams"),
                ("AntihistamineDose", "Count"),
                ("AntifungalDose", "Milliliters"),
                ("AntifungalDose", "Milligrams"),
                ("AntifungalDose", "Grams"),
                ("AntifungalDose", "Count"),
                ("AntiepilepticDose", "Milliliters"),
                ("AntiepilepticDose", "Milligrams"),
                ("AntiepilepticDose", "Grams"),
                ("AntiepilepticDose", "Count"),
                ("AnxiolyticDose", "Milliliters"),
                ("AnxiolyticDose", "Milligrams"),
                ("AnxiolyticDose", "Grams"),
                ("AnxiolyticDose", "Count"),
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
                new MetaTag { Name = "Rescue", Description = "Rescued animal", IsActive = true },
                new MetaTag { Name = "Senior", Description = "Senior animal", IsActive = true }
            };

            public static readonly List<SubjectType> SubjectTypes = new()
            {
                new SubjectType { Clade = "Breed", Name = "Mixed Breed", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Affenpinscher", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Afghan Hound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Aidi", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Airedale Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Akbash Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Akita", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Alano Español", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Alaskan Husky", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Alaskan Klee Kai", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Alaskan Malamute", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "American Bulldog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "American Bully", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "American English Coonhound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "American Eskimo Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "American Foxhound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "American Hairless Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "American Leopard Hound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "American Pit Bull Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "American Staffordshire Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "American Water Spaniel", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Anatolian Shepherd Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Appenzeller Sennenhund", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Ariegeois", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Armant", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Australian Cattle Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Australian Kelpie", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Australian Shepherd", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Australian Stumpy Tail Cattle Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Australian Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Azawakh", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Barbet", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Basenji", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Basset Bleu de Gascogne", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Basset Fauve de Bretagne", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Basset Hound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Bavarian Mountain Hound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Beagle", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Bearded Collie", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Beauceron", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Bedlington Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Belgian Laekenois", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Belgian Malinois", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Belgian Sheepdog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Belgian Tervuren", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Bergamasco Sheepdog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Berger Picard", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Bernese Mountain Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Bichon Frise", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Biewer Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Black and Tan Coonhound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Black Russian Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Bloodhound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Bluetick Coonhound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Boerboel", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Bolognese", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Border Collie", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Border Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Borzoi", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Boston Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Bouvier des Flandres", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Boxer", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Boykin Spaniel", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Bracco Italiano", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Braque du Bourbonnais", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Braque Francais", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Brazilian Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Briard", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Brittany", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Broholmer", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Brussels Griffon", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Bucovina Shepherd", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Bull Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Bulldog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Bullmastiff", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Cairn Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Canaan Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Canadian Eskimo Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Cane Corso", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Cardigan Welsh Corgi", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Carolina Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Carpathian Shepherd Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Catahoula Leopard Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Caucasian Ovcharka", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Cavalier King Charles Spaniel", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Central Asian Shepherd Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Cesky Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Chesapeake Bay Retriever", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Chihuahua", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Chinese Crested", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Chinese Shar-Pei", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Chinook", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Chow Chow", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Cirneco dell’Etna", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Clumber Spaniel", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Cocker Spaniel", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Collie", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Coton de Tulear", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Croatian Sheepdog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Curly-Coated Retriever", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Czechoslovakian Vlcak", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Dachshund", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Dalmatian", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Dandie Dinmont Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Danish-Swedish Farmdog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Deutscher Wachtelhund", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Doberman Pinscher", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Dogo Argentino", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Dogue de Bordeaux", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Drentsche Patrijshond", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Drever", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Dutch Shepherd", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "English Cocker Spaniel", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "English Foxhound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "English Setter", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "English Shepherd", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "English Springer Spaniel", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "English Toy Spaniel", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Entlebucher Mountain Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Estrela Mountain Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Eurasier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Field Spaniel", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Fila Brasileiro", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Finnish Lapphund", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Finnish Spitz", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Flat-Coated Retriever", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "French Bulldog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "French Spaniel", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "German Longhaired Pointer", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "German Pinscher", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "German Shepherd Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "German Shorthaired Pointer", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "German Wirehaired Pointer", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Giant Schnauzer", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Glen of Imaal Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Golden Retriever", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Gordon Setter", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Grand Basset Griffon Vendéen", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Great Dane", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Great Pyrenees", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Greater Swiss Mountain Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Greyhound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Hamiltonstövare", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Hanoverian Scenthound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Harrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Havanese", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Hokkaido", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Hovawart", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Ibizan Hound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Icelandic Sheepdog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Irish Red and White Setter", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Irish Setter", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Irish Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Irish Water Spaniel", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Irish Wolfhound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Italian Greyhound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Jack Russell Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Japanese Chin", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Japanese Spitz", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Jindo", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Kai Ken", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Kangal Shepherd Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Karelian Bear Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Keeshond", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Kerry Blue Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Kishu Ken", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Komondor", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Kooikerhondje", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Kuvasz", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Labrador Retriever", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Lagotto Romagnolo", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Lakeland Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Lancashire Heeler", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Leonberger", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Lhasa Apso", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Löwchen", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Maltese", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Manchester Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Maremma Sheepdog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Mastiff", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Miniature American Shepherd", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Miniature Bull Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Miniature Pinscher", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Miniature Schnauzer", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Mudi", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Neapolitan Mastiff", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Newfoundland", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Norfolk Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Norrbottenspets", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Norwegian Buhund", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Norwegian Elkhound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Norwegian Lundehund", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Norwich Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Nova Scotia Duck Tolling Retriever", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Old English Sheepdog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Otterhound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Papillon", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Parson Russell Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Patterdale Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Pekingese", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Pembroke Welsh Corgi", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Perro de Presa Canario", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Peruvian Inca Orchid", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Petit Basset Griffon Vendéen", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Pharaoh Hound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Plott Hound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Podengo Português", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Pointer", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Polish Lowland Sheepdog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Pomeranian", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Poodle", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Porcelaine", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Portuguese Sheepdog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Portuguese Water Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Pug", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Puli", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Pumi", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Pyrenean Mastiff", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Pyrenean Shepherd", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Rafeiro do Alentejo", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Rat Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Redbone Coonhound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Rhodesian Ridgeback", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Rottweiler", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Russell Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Russian Toy", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Saint Bernard", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Saluki", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Samoyed", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Schapendoes", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Schipperke", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Scottish Deerhound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Scottish Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Sealyham Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Shetland Sheepdog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Shiba Inu", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Shih Tzu", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Shiloh Shepherd", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Siberian Husky", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Silky Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Skye Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Sloughi", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Slovakian Wirehaired Pointer", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Small Munsterlander", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Soft Coated Wheaten Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Spanish Mastiff", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Spanish Water Dog", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Spinone Italiano", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Stabyhoun", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Staffordshire Bull Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Standard Schnauzer", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Sussex Spaniel", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Swedish Lapphund", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Swedish Vallhund", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Teddy Roosevelt Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Thai Ridgeback", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Tibetan Mastiff", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Tibetan Spaniel", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Tibetan Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Tornjak", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Tosa", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Toy Fox Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Transylvanian Hound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Treeing Feist", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Treeing Tennessee Brindle", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Treeing Walker Coonhound", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Vizsla", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Weimaraner", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Welsh Springer Spaniel", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Welsh Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "West Highland White Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Wetterhoun", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Whippet", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "White Shepherd", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Wire Fox Terrier", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Wirehaired Pointing Griffon", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Wirehaired Vizsla", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Xoloitzcuintli", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Yakutian Laika", ScientificName = "Canis lupus familiaris" },
                new SubjectType { Clade = "Breed", Name = "Yorkshire Terrier", ScientificName = "Canis lupus familiaris" }
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