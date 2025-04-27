// Services/EnergyCalculationService.cs
using BiermanTech.CriticalDog.ViewModels;

namespace BiermanTech.CriticalDog.Services.Interfaces
{
    public interface IEnergyCalculationService
    {
        Task<EnergyCalculationResult> CalculateEnergyRequirementsAsync(EnergyCalculationInput input);
        DateTime? ProjectIdealWeightDate(decimal idealWeight, decimal lastRecordedWeight, DateTime lastRecordedDate, decimal weeklyWeightLoss);
    }
}