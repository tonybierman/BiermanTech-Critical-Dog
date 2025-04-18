using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Helpers.BiermanTech.CriticalDog.Helpers
{
    public interface IMetricValueTransformProvider
    {
        IEnumerable<SelectListItem> GetSelectListItems();
        string? GetTransformedValue(int value);
    }
}