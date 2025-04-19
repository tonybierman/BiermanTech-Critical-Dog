using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Reports.Columns
{
    public interface IMetricValueTransformProvider
    {
        IEnumerable<SelectListItem> GetSelectListItems();
        string? GetTransformedValue(int value);
    }
}