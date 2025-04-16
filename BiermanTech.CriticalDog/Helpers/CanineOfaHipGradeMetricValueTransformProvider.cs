using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Helpers
{
    public class CanineOfaHipGradeMetricValueTransformProvider : IMetricValueTransformProvider
    {
        public IEnumerable<SelectListItem> GetSelectListItems()
        {
            return SelectListHelper.GetOfaHipGradesSelectList();
        }

        public string? GetTransormedValue(int value)
        {
            return ((CanineOfaHipGradeEnum)value).GetDisplayName();
        }
    }
}
