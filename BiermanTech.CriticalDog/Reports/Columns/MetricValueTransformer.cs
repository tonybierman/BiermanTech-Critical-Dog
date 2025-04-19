using BiermanTech.CriticalDog.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace BiermanTech.CriticalDog.Reports.Columns
{
    public class MetricValueTransformer<T> : IMetricValueTransformProvider where T : struct, Enum
    {
        public IEnumerable<SelectListItem> GetSelectListItems()
        {
            var selectList = EnumHelper.ToSelectList<T>(selected: null);
            return selectList.Select(item => new SelectListItem
            {
                Value = item.Value,
                Text = item.Text,
                Selected = item.Selected
            });
        }

        public string? GetTransformedValue(int value)
        {
            if (!Enum.IsDefined(typeof(T), value))
            {
                return null; // or return "Unknown" if preferred
            }

            var enumValue = (T)(object)value; // Safe cast after validation
            return EnumHelper.GetEnumDisplayName(enumValue);
        }
    }
}