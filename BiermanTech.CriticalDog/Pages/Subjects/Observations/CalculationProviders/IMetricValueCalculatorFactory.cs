namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.CalculationProviders
{
    public interface IMetricValueCalculatorFactory
    {
        IMetricValueCalculatorProvider? GetCalculator(string? observationDefinitionName);
    }
}