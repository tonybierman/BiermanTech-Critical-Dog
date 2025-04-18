namespace BiermanTech.CriticalDog.Helpers
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the body condition scores (BCS) for assessing a dog's weight and health status.
    /// Based on the 9-point scale used in veterinary practice.
    /// </summary>
    public enum BodyConditionScoreEnum
    {
        /// <summary>
        /// No specific body condition (invalid for BCS assessment).
        /// </summary>
        [Display(Name = "None")]
        None = 0,

        /// <summary>
        /// Emaciated: Severe muscle loss, no palpable fat, bones highly visible.
        /// </summary>
        [Display(Name = "Emaciated")]
        Emaciated = 1,

        /// <summary>
        /// Very Thin: Some muscle loss, minimal fat, ribs and spine easily visible.
        /// </summary>
        [Display(Name = "Very Thin")]
        VeryThin = 2,

        /// <summary>
        /// Thin: Ribs visible, slight fat cover, obvious waist and abdominal tuck.
        /// </summary>
        [Display(Name = "Thin")]
        Thin = 3,

        /// <summary>
        /// Underweight: Ribs easily palpable, minimal fat, prominent waist and tuck.
        /// </summary>
        [Display(Name = "Underweight")]
        Underweight = 4,

        /// <summary>
        /// Ideal: Ribs palpable with slight fat cover, well-defined waist and tuck.
        /// </summary>
        [Display(Name = "Ideal")]
        Ideal = 5,

        /// <summary>
        /// Overweight: Ribs palpable with difficulty, noticeable fat, reduced waist.
        /// </summary>
        [Display(Name = "Overweight")]
        Overweight = 6,

        /// <summary>
        /// Heavy: Ribs difficult to palpate, thick fat cover, minimal waist.
        /// </summary>
        [Display(Name = "Heavy")]
        Heavy = 7,

        /// <summary>
        /// Obese: Ribs not palpable, heavy fat deposits, no waist or tuck.
        /// </summary>
        [Display(Name = "Obese")]
        Obese = 8,

        /// <summary>
        /// Severely Obese: Extreme fat deposits, significant health risks.
        /// </summary>
        [Display(Name = "Severely Obese")]
        SeverelyObese = 9
    }
}
