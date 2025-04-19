using System.ComponentModel.DataAnnotations;

namespace BiermanTech.CriticalDog.Reports.Columns
{
    /// <summary>
    /// Defines the life stages and conditions of a dog that affect its Maintenance Energy Requirement (MER) factors.
    /// </summary>
    public enum LifeStageFactorsEnum
    {
        /// <summary>
        /// No specific life stage or condition (invalid for MER calculation).
        /// </summary>
        [Display(Name = "None")]
        None = 0,

        /// <summary>
        /// Neutered adult dog.
        /// </summary>
        [Display(Name = "Neutered Adult")]
        NeuteredAdult = 1,

        /// <summary>
        /// Intact (non-neutered) adult dog.
        /// </summary>
        [Display(Name = "Intact Adult")]
        IntactAdult = 2,

        /// <summary>
        /// Inactive or obesity-prone dog.
        /// </summary>
        [Display(Name = "Inactive/Obese Prone")]
        InactiveObeseProne = 3,

        /// <summary>
        /// Dog on a weight loss plan.
        /// </summary>
        [Display(Name = "Weight Loss")]
        WeightLoss = 4,

        /// <summary>
        /// Dog requiring weight gain.
        /// </summary>
        [Display(Name = "Weight Gain")]
        WeightGain = 5,

        /// <summary>
        /// Active working dog with high energy needs.
        /// </summary>
        [Display(Name = "Active Working Dog")]
        ActiveWorkingDog = 6,

        /// <summary>
        /// Puppy aged 0 to 4 months.
        /// </summary>
        [Display(Name = "Puppy (0-4 Months)")]
        Puppy0To4Months = 7,

        /// <summary>
        /// Puppy aged 4 months to adulthood.
        /// </summary>
        [Display(Name = "Puppy (4 Months to Adult)")]
        Puppy4MonthsToAdult = 8,

        /// <summary>
        /// Pregnant (gestating) dog.
        /// </summary>
        [Display(Name = "Gestation")]
        Gestation = 9,

        /// <summary>
        /// Lactating (nursing) dog.
        /// </summary>
        [Display(Name = "Lactation")]
        Lactation = 10
    }
}