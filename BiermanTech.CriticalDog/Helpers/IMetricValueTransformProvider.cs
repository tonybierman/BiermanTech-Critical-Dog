using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Helpers
{
    public interface IMetricValueTransformProvider
    {
        IEnumerable<SelectListItem> GetSelectListItems();

        public string GetTransormedValue(int value);
    }
}
