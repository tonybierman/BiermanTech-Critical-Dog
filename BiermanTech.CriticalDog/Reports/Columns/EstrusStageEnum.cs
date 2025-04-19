namespace BiermanTech.CriticalDog.Helpers
{
    using System.ComponentModel.DataAnnotations;

    namespace BiermanTech.CriticalDog.Helpers
    {
        /// <summary>
        /// Defines the stages of a dog's estrus cycle.
        /// </summary>
        public enum EstrusStageEnum
        {
            /// <summary>
            /// No specific estrus stage (invalid for estrus-related calculations).
            /// </summary>
            [Display(Name = "None")]
            None = 0,

            /// <summary>
            /// Proestrus stage, characterized by the beginning of heat with vaginal bleeding and swelling.
            /// </summary>
            [Display(Name = "Proestrus")]
            Proestrus = 1,

            /// <summary>
            /// Estrus stage, the fertile period when the female is receptive to mating.
            /// </summary>
            [Display(Name = "Estrus")]
            Estrus = 2,

            /// <summary>
            /// Diestrus stage, post-ovulation period where the body prepares for pregnancy or returns to normal.
            /// </summary>
            [Display(Name = "Diestrus")]
            Diestrus = 3,

            /// <summary>
            /// Anestrus stage, the resting phase with no reproductive activity.
            /// </summary>
            [Display(Name = "Anestrus")]
            Anestrus = 4
        }
    }
}
