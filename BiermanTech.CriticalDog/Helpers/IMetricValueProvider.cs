using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Helpers
{
    public interface IMetricValueProvider
    {
        IEnumerable<SelectListItem> GetSelectListItems();

        public string GetDisplayName(int value);
    }
}
