// Services/EnergyCalculationService.cs
using BiermanTech.CriticalDog.ViewModels;

namespace BiermanTech.CriticalDog.Services.Interfaces
{
    public interface IEnergyCalculationService
    {
        Task<EnergyCalculationResult> CalculateEnergyRequirementsAsync(EnergyCalculationInput input);
    }
}