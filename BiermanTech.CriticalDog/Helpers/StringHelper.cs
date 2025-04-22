using System.Text;

namespace BiermanTech.CriticalDog.Helpers
{
    /// <summary>
    /// Provides utility methods for string manipulation.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Splits a PascalCase string into a space-separated string by inserting a space before uppercase letters that indicate a new word.
        /// Consecutive uppercase letters (e.g., acronyms) are treated as a single unit unless followed by a lowercase letter.
        /// </summary>
        /// <param name="input">The PascalCase string to split. Can be null or empty.</param>
        /// <returns>
        /// A string with spaces inserted before uppercase letters that start a new word, or the original input if it is null or empty.
        /// For example, "HelloWorld" becomes "Hello World", and "XMLParser" becomes "XML Parser".
        /// </returns>
        /// <remarks>
        /// This method preserves the first character as-is and groups consecutive uppercase letters together (e.g., "XML" stays intact).
        /// A space is added before an uppercase letter if it follows a lowercase letter or marks the end of an acronym followed by a lowercase letter.
        /// If the input is null or an empty string, it is returned unchanged.
        /// </remarks>
        /// <example>
        /// <code>
        /// string result = StringHelper.SplitPascalCase("HelloWorld");
        /// // result is "Hello World"
        /// 
        /// string result2 = StringHelper.SplitPascalCase("XMLParser");
        /// // result2 is "XML Parser"
        /// 
        /// string result3 = StringHelper.SplitPascalCase(null);
        /// // result3 is null
        /// </code>
        /// </example>
        public static string SplitPascalCase(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var result = new StringBuilder();
            result.Append(input[0]); // Keep first char as is

            for (int i = 1; i < input.Length; i++)
            {
                // Add a space if current char is uppercase and either:
                // 1. Previous char is lowercase (e.g., "lW" in "HelloWorld")
                // 2. Previous char is uppercase and next char (if exists) is lowercase (e.g., "L" in "XMLParser")
                // 3. Previous char is not a space to avoid double spaces
                if (char.IsUpper(input[i]) &&
                    input[i - 1] != ' ' &&
                    (char.IsLower(input[i - 1]) ||
                     (i + 1 < input.Length && char.IsLower(input[i + 1]))))
                {
                    result.Append(' ');
                }
                result.Append(input[i]);
            }

            return result.ToString();
        }

        public static string Slugify(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Use SplitPascalCase to get space-separated string
            string splitResult = SplitPascalCase(input);

            // Replace spaces with hyphens and convert to lowercase
            return splitResult.Replace(" ", "-").ToLower();
        }
    }
}
