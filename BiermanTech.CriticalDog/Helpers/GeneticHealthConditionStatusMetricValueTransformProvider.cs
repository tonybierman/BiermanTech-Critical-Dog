using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Helpers
{
    public class GeneticHealthConditionStatusMetricValueTransformProvider : IMetricValueTransformProvider
    {
        public IEnumerable<SelectListItem> GetSelectListItems()
        {
            return SelectListHelper.CanineGeneticHealthConditionStatusSelectList();
        }

        public string? GetTransormedValue(int value)
        {
            return ((GeneticHealthConditionStatusEnum)value).GetDisplayName();
        }
    }
}
