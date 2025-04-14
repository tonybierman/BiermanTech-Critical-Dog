using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Helpers
{
    public class OfaHipGradeMetricValueTransformProvider : IMetricValueTransformProvider
    {
        public IEnumerable<SelectListItem> GetSelectListItems()
        {
            return SelectListHelper.GetOfaHipGradesSelectList();
        }

        public string? GetTransormedValue(int value)
        {
            return ((OfaHipGradeEnum)value).GetDisplayName();
        }
    }
}
