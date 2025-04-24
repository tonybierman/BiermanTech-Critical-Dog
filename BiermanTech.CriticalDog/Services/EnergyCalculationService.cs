// Services/EnergyCalculationService.cs
using BiermanTech.CanineHealth;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using System;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Services
{
    public class EnergyCalculationService : IEnergyCalculationService
    {
        private readonly IUnitConverter _units;

        public EnergyCalculationService(IUnitConverter units)
        {
            _units = units ?? throw new ArgumentNullException(nameof(units));
        }

        public async Task<EnergyCalculationResult> CalculateEnergyRequirementsAsync(EnergyCalculationInput input)
        {
            var defaultResult = new EnergyCalculationResult
            {
                IsValid = false,
                WeightInKgs = 0,
                WeightInLbs = 0,
                Rer = 0,
                MeanMer = 0,
                LifeStage = default
            };

            if (input?.WeightMetricValue == null ||
                input?.UnitName == null ||
                input?.LifeStageMetricValue == null)
            {
                return defaultResult;
            }

            decimal? weightMetricValue = input.WeightMetricValue;
            double? weight = weightMetricValue.HasValue ? (double)weightMetricValue.Value : null;

            decimal? lifeStageMetricValue = input.LifeStageMetricValue;
            bool isValidLifeStage = false;
            LifeStageFactorsEnum lifeStage = default;

            if (lifeStageMetricValue.HasValue)
            {
                try
                {
                    int enumValue = (int)lifeStageMetricValue.Value;
                    if (Enum.IsDefined(typeof(LifeStageFactorsEnum), enumValue))
                    {
                        lifeStage = (LifeStageFactorsEnum)enumValue;
                        isValidLifeStage = true;
                    }
                }
                catch
                {
                    isValidLifeStage = false;
                }
            }

            if (!weight.HasValue || !isValidLifeStage)
            {
                return defaultResult;
            }

            try
            {
                double weightInKgs = await _units.ConvertAsync(
                    input.UnitName,
                    "Kilograms",
                    weight.Value);

                double weightInLbs = await _units.ConvertAsync(
                    input.UnitName,
                    "Pounds",
                    weight.Value);

                (double rer, double lowerMer, double upperMer) = CanineMerCalculator.CalculateMer(lifeStage, weightInKgs);
                double meanMer = CanineMerCalculator.CalculateMeanMer(lifeStage, weightInKgs);

                return new EnergyCalculationResult
                {
                    IsValid = true,
                    WeightInKgs = weightInKgs,
                    WeightInLbs = weightInLbs,
                    Rer = rer,
                    MeanMer = meanMer,
                    LifeStage = lifeStage
                };
            }
            catch
            {
                // Log error if needed
                return defaultResult;
            }
        }
    }
}