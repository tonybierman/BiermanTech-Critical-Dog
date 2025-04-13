namespace BiermanTech.CriticalDog.Helpers
{
    /// <summary>
    /// Defines the life stages and conditions of a dog that affect its Maintenance Energy Requirement (MER) factors.
    /// </summary>
    public enum LifeStageFactorsEnum
    {
        /// <summary>
        /// No specific life stage or condition (invalid for MER calculation).
        /// </summary>
        None = 0,

        /// <summary>
        /// Neutered adult dog.
        /// </summary>
        NeuteredAdult = 1,

        /// <summary>
        /// Intact (non-neutered) adult dog.
        /// </summary>
        IntactAdult = 2,

        /// <summary>
        /// Inactive or obesity-prone dog.
        /// </summary>
        InactiveObeseProne = 3,

        /// <summary>
        /// Dog on a weight loss plan.
        /// </summary>
        WeightLoss = 4,

        /// <summary>
        /// Dog requiring weight gain.
        /// </summary>
        WeightGain = 5,

        /// <summary>
        /// Active working dog with high energy needs.
        /// </summary>
        ActiveWorkingDog = 6,

        /// <summary>
        /// Puppy aged 0 to 4 months.
        /// </summary>
        Puppy0To4Months = 7,

        /// <summary>
        /// Puppy aged 4 months to adulthood.
        /// </summary>
        Puppy4MonthsToAdult = 8,

        /// <summary>
        /// Pregnant (gestating) dog.
        /// </summary>
        Gestation = 9,

        /// <summary>
        /// Lactating (nursing) dog.
        /// </summary>
        Lactation = 10
    }
}
