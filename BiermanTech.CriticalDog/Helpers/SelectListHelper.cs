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
        public static SelectList GetLifeStageFactorsSelectList(CanineLifeStageFactorsEnum? selected = null)
        {
            var enumValues = Enum.GetValues(typeof(CanineLifeStageFactorsEnum))
                .Cast<CanineLifeStageFactorsEnum>()
                .Where(e => e != CanineLifeStageFactorsEnum.None) // Exclude 'None' if not desired
                .Select(e => new
                {
                    Value = ((int)e).ToString(),
                    Text = GetEnumDisplayName(e)
                })
                .ToList();

            return new SelectList(enumValues, "Value", "Text", selected?.ToString());
        }

        public static SelectList GetOfaHipGradesSelectList(CanineOfaHipGradeEnum? selected = null)
        {
            var enumValues = Enum.GetValues(typeof(CanineOfaHipGradeEnum))
                .Cast<CanineOfaHipGradeEnum>()
                .Where(e => e != CanineOfaHipGradeEnum.None) // Exclude 'None' if not desired
                .Select(e => new
                {
                    Value = ((int)e).ToString(),
                    Text = GetEnumDisplayName(e)
                })
                .ToList();

            return new SelectList(enumValues, "Value", "Text", selected?.ToString());
        }

        internal static IEnumerable<SelectListItem> CanineGeneticHealthConditionStatusSelectList(CanineGeneticHealthConditionStatusEnum? selected = null)
        {
            var enumValues = Enum.GetValues(typeof(CanineGeneticHealthConditionStatusEnum))
                .Cast<CanineGeneticHealthConditionStatusEnum>()
                .Where(e => e != CanineGeneticHealthConditionStatusEnum.None) // Exclude 'None' if not desired
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
