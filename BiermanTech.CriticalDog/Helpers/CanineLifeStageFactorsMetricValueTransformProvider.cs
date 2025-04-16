using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Helpers
{
    public class CanineLifeStageFactorsMetricValueTransformProvider : IMetricValueTransformProvider
    {
        public IEnumerable<SelectListItem> GetSelectListItems()
        {
            return SelectListHelper.GetLifeStageFactorsSelectList();
        }

        public string? GetTransormedValue(int value)
        {
            return ((CanineLifeStageFactorsEnum)value).GetDisplayName();
        }
    }
}
