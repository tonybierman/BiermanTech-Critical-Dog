using BiermanTech.CanineHealth;

namespace BiermanTech.CriticalDog.ViewModels
{
    public record EnergyCalculationResult
    {
        public bool IsValid { get; init; }
        public double WeightInKgs { get; init; }
        public double WeightInLbs { get; init; }
        public double Rer { get; init; }
        public double MeanMer { get; init; }
        public LifeStageFactorsEnum LifeStage { get; init; }
    }
}
