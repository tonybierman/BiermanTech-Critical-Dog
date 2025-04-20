using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public class UnitConverter : IUnitConverter
{
    private readonly Dictionary<(string From, string To), Func<double, double>> conversionRules;

    public UnitConverter()
    {
        conversionRules = new Dictionary<(string From, string To), Func<double, double>>
        {
            // Mass conversions
            { ("Kilograms", "Pounds"), value => value * 2.20462262185 },
            { ("Pounds", "Kilograms"), value => value / 2.20462262185 },
            { ("Kilograms", "Ounces"), value => value * 35.2739619496 },
            { ("Ounces", "Kilograms"), value => value / 35.2739619496 },
            { ("Pounds", "Ounces"), value => value * 16 },
            { ("Ounces", "Pounds"), value => value / 16 },
            { ("Kilograms", "Grams"), value => value * 1000 },
            { ("Grams", "Kilograms"), value => value / 1000 },
            { ("Kilograms", "Milligrams"), value => value * 1000000 },
            { ("Milligrams", "Kilograms"), value => value / 1000000 },
            { ("Grams", "Milligrams"), value => value * 1000 },
            { ("Milligrams", "Grams"), value => value / 1000 },
            { ("Pounds", "Grams"), value => value * 453.59237 },
            { ("Grams", "Pounds"), value => value / 453.59237 },
            { ("Ounces", "Grams"), value => value * 28.349523125 },
            { ("Grams", "Ounces"), value => value / 28.349523125 },
            { ("Pounds", "Milligrams"), value => value * 453592.37 },
            { ("Milligrams", "Pounds"), value => value / 453592.37 },
            { ("Ounces", "Milligrams"), value => value * 28349.523125 },
            { ("Milligrams", "Ounces"), value => value / 28349.523125 },

            // Temperature conversions
            { ("DegreesCelsius", "DegreesFahrenheit"), value => (value * 9 / 5) + 32 },
            { ("DegreesFahrenheit", "DegreesCelsius"), value => (value - 32) * 5 / 9 },

            // Time conversions
            { ("Days", "Minutes"), value => value * 1440 },
            { ("Minutes", "Days"), value => value / 1440 },

            // Volume (no conversion needed for milliliters as it's standalone)
            { ("Milliliters", "Milliliters"), value => value },

            // Non-convertible units (1:1 for same unit)
            { ("Count", "Count"), value => value },
            { ("BeatsPerMinute", "BeatsPerMinute"), value => value },
            { ("Percentage", "Percentage"), value => value },
            { ("Score", "Score"), value => value },
            { ("Kilocalories", "Kilocalories"), value => value }
        };
    }

    public async Task<double> ConvertAsync(string fromUnit, string toUnit, double value)
    {
        if (string.IsNullOrEmpty(fromUnit) || string.IsNullOrEmpty(toUnit))
        {
            throw new ArgumentException("Unit names cannot be null or empty.");
        }

        // Check if units are the same (no conversion needed)
        if (fromUnit == toUnit)
        {
            return value;
        }

        // Try direct conversion
        if (conversionRules.TryGetValue((fromUnit, toUnit), out var conversion))
        {
            return conversion(value);
        }

        // No direct conversion found
        throw new InvalidOperationException($"No conversion available from {fromUnit} to {toUnit}.");
    }
}