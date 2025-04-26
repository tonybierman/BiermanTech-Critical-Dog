using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Services.Interfaces
{
    public interface IMetricValueTransformProvider
    {
        IEnumerable<SelectListItem> GetSelectListItems();
        string? GetTransformedValue(int value);
    }
}