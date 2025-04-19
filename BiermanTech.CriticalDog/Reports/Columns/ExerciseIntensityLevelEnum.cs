namespace BiermanTech.CriticalDog.Reports.Columns
{
    using System.ComponentModel.DataAnnotations;

    namespace BiermanTech.CriticalDog.Reports.Columns
    {
        /// <summary>
        /// Defines simplified exercise intensity levels for evaluating physical activity in dogs, based on veterinary guidelines.
        /// </summary>
        public enum ExerciseIntensityLevelEnum
        {
            /// <summary>
            /// No specific exercise intensity assigned (invalid for evaluation).
            /// </summary>
            [Display(Name = "None", Description = "No specific exercise intensity assigned (invalid for evaluation).")]
            None = 0,

            /// <summary>
            /// Low-intensity activity, such as slow walking or light play, suitable for senior or recovering dogs.
            /// </summary>
            [Display(Name = "Low", Description = "Low-intensity activity, such as slow walking or light play, suitable for senior or recovering dogs.")]
            Low = 1,

            /// <summary>
            /// Moderate activity, such as brisk walking or casual play, appropriate for most healthy adult dogs.
            /// </summary>
            [Display(Name = "Moderate", Description = "Moderate activity, such as brisk walking or casual play, appropriate for most healthy adult dogs.")]
            Moderate = 2,

            /// <summary>
            /// High-intensity activity, such as running, fetch, or agility, for fit and active dogs.
            /// </summary>
            [Display(Name = "High", Description = "High-intensity activity, such as running, fetch, or agility, for fit and active dogs.")]
            High = 3,

            /// <summary>
            /// Very high-intensity activity, such as sprinting or competitive sports, for highly fit or working dogs.
            /// </summary>
            [Display(Name = "Very High", Description = "Very high-intensity activity, such as sprinting or competitive sports, for highly fit or working dogs.")]
            VeryHigh = 4
        }
    }
}
