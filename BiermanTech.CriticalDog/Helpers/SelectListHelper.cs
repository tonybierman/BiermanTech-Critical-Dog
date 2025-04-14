using BiermanTech.CriticalDog.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace BiermanTech.CriticalDog.Helpers
{
    public static class SelectListHelper
    {
        public static SelectList GetLifeStageFactorsSelectList(LifeStageFactorsEnum? selected = null)
        {
            var enumValues = Enum.GetValues(typeof(LifeStageFactorsEnum))
                .Cast<LifeStageFactorsEnum>()
                .Where(e => e != LifeStageFactorsEnum.None) // Exclude 'None' if not desired
                .Select(e => new
                {
                    Value = ((int)e).ToString(),
                    Text = GetEnumDisplayName(e)
                })
                .ToList();

            return new SelectList(enumValues, "Value", "Text", selected?.ToString());
        }

        public static SelectList GetOfaHipGradesSelectList(OfaHipGradeEnum? selected = null)
        {
            var enumValues = Enum.GetValues(typeof(OfaHipGradeEnum))
                .Cast<OfaHipGradeEnum>()
                .Where(e => e != OfaHipGradeEnum.None) // Exclude 'None' if not desired
                .Select(e => new
                {
                    Value = ((int)e).ToString(),
                    Text = GetEnumDisplayName(e)
                })
                .ToList();

            return new SelectList(enumValues, "Value", "Text", selected?.ToString());
        }

        private static string GetEnumDisplayName(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var displayAttribute = fieldInfo?.GetCustomAttribute<DisplayAttribute>();
            return displayAttribute?.Name ?? value.ToString();
        }
    }
}
