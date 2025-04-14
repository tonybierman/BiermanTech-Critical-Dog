using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Helpers
{
    public class LifeStageFactorsMetricValueProvider : IMetricValueProvider
    {
        public IEnumerable<SelectListItem> GetSelectListItems()
        {
            return SelectListHelper.GetLifeStageFactorsSelectList();
        }

        public string GetDisplayName(int value)
        {
            return ((LifeStageFactorsEnum)value).GetDisplayName();
        }
    }
}
