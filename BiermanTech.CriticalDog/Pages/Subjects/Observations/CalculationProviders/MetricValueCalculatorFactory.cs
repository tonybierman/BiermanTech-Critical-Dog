using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Helpers.BiermanTech.CriticalDog.Helpers;
using System;
using System.Collections.Generic;
using UniversalReportCore;
using UniversalReportCore.PageMetadata;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.CalculationProviders
{
    public class MetricValueCalculatorFactory : IMetricValueCalculatorFactory
    {
        private readonly IEnumerable<IMetricValueCalculatorProvider> _providers;

        public MetricValueCalculatorFactory(IEnumerable<IMetricValueCalculatorProvider> providers)
        {
            _providers = providers;
        }

        public IMetricValueCalculatorProvider? GetCalculator(string? observationDefinitionName)
        {
            var calculator = _providers.FirstOrDefault(p => p.Slug == observationDefinitionName);
            //if (calculator == null)
            //{
            //    throw new InvalidOperationException($"Unsupported calculator type: {observationDefinitionName}");
            //}

            return calculator;
        }
    }
}