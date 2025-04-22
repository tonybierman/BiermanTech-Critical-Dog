namespace BiermanTech.CriticalDog.ViewModels
{
    public record EnergyCalculationInput
    {
        public decimal? WeightMetricValue { get; init; }
        public string? UnitName { get; init; }
        public decimal? LifeStageMetricValue { get; init; }
    }
}
