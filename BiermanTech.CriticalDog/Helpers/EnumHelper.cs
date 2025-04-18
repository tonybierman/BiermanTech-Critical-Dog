using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Helpers
{
    public static class EnumHelper
    {
        /// <summary>
        /// Creates a SelectList from any enum type, excluding specified values and using Display names.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="selected">The selected enum value (nullable).</param>
        /// <param name="exclude">Optional array of enum values to exclude (e.g., None).</param>
        /// <returns>A SelectList for use in dropdowns.</returns>
        public static SelectList ToSelectList<TEnum>(TEnum? selected = null, params TEnum[] exclude)
            where TEnum : struct, Enum
        {
            var enumValues = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Where(e => !exclude.Contains(e)) // Exclude specified values
                .Select(e => new
                {
                    Value = ((int)(object)e).ToString(), // Cast to int for value
                    Text = GetEnumDisplayName(e)
                })
                .ToList();

            return new SelectList(enumValues, "Value", "Text", selected?.ToString());
        }

        /// <summary>
        /// Retrieves the Display Name of an enum value, falling back to the enum's name if no Display attribute is defined.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>The Display Name or the enum value's name.</returns>
        public static string GetEnumDisplayName<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            var displayAttribute = fieldInfo?.GetCustomAttribute<DisplayAttribute>();
            return displayAttribute?.Name ?? enumValue.ToString();
        }
    }
}