using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Helpers.BiermanTech.CriticalDog.Helpers;
using System;
using System.Collections.Generic;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.CalculationProviders
{
    public static class MetricValueCalculatorFactory
    {
        // Dictionary to map observation type names to provider instances
        private static readonly Dictionary<string, IMetricValueCalculatorProvider> _providers = new()
        {
            { "DailyCaloricIntake", new DailyCaloricIntakeCalculator() }
        };

        /// <summary>
        /// Gets the appropriate IMetricValueCalculatorProvider.
        /// </summary>
        /// <param name="observationDefinitionName">The observation definition name.</param>
        /// <returns>An IMetricValueCalculatorProvider or null if no matching provider is found.</returns>
        public static IMetricValueCalculatorProvider? GetProvider(string? observationDefinitionName)
        {
            if (string.IsNullOrEmpty(observationDefinitionName))
            {
                return null;
            }

            return _providers.TryGetValue(observationDefinitionName, out var provider) ? provider : null;
        }
    }
}