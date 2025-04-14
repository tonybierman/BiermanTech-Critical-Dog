using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Helpers
{
    public class LifeStageFactorsMetricValueTransformProvider : IMetricValueTransformProvider
    {
        public IEnumerable<SelectListItem> GetSelectListItems()
        {
            return SelectListHelper.GetLifeStageFactorsSelectList();
        }

        public string? GetTransormedValue(int value)
        {
            return ((LifeStageFactorsEnum)value).GetDisplayName();
        }
    }
}
