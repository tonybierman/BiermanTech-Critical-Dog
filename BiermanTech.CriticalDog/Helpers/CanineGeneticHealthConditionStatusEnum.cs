using System.ComponentModel.DataAnnotations;

namespace BiermanTech.CriticalDog.Helpers
{
    /// <summary>
    /// Covers the possible outcomes for canine genetic health conditions
    /// </summary>
    public enum CanineGeneticHealthConditionStatusEnum
    {
        /// <summary>
        /// The dog has not been evaluate for the condition of the result is unknown.
        /// </summary>
        [Display(Name = "None", Description = "The dog has not been evaluate for the condition of the result is unknown.")]
        None = 0,

        /// <summary>
        /// The dog does not have the genetic variant(s) associated with the condition and is unlikely to develop or pass it on.
        /// </summary>
        [Display(Name = "Clear", Description = "The dog does not have the genetic variant(s) associated with the condition and is unlikely to develop or pass it on.")]
        Excellent = 1,

        /// <summary>
        /// The dog has one copy of a genetic variant associated with a condition but is not expected to develop symptoms.
        /// </summary>
        [Display(Name = "Carrier", Description = "The dog has one copy of a genetic variant associated with a condition but is not expected to develop symptoms.")]
        Carrier = 2,

        /// <summary>
        /// The dog has one or two copies of a genetic variant associated with a specific health condition, indicating a higher likelihood of developing it.
        /// </summary>
        [Display(Name = "AtRisk", Description = "The dog has one or two copies of a genetic variant associated with a specific health condition, indicating a higher likelihood of developing it.")]
        AtRisk = 3
    }
}

