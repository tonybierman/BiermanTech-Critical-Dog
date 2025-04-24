
public interface IUnitConverter
{
    Task<double> ConvertAsync(string fromUnit, string toUnit, double value);
}