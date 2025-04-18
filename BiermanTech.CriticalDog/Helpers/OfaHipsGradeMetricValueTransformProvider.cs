using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Helpers
{
    public class OfaHipsGradeMetricValueTransformProvider : IMetricValueTransformProvider
    {
        public IEnumerable<SelectListItem> GetSelectListItems()
        {
            return SelectListHelper.GetOfaHipGradesSelectList();
        }

        public string? GetTransormedValue(int value)
        {
            return ((OfaHipsGradeEnum)value).GetDisplayName();
        }
    }
}
