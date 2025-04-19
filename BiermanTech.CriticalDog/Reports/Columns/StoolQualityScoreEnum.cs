namespace BiermanTech.CriticalDog.Reports.Columns
{
    using System.ComponentModel.DataAnnotations;

    namespace BiermanTech.CriticalDog.Reports.Columns
    {
        /// <summary>
        /// Defines the canine stool quality scores for evaluating fecal consistency in dogs.
        /// </summary>
        public enum StoolQualityScoreEnum
        {
            /// <summary>
            /// No specific stool quality assigned (invalid for evaluation).
            /// </summary>
            [Display(Name = "None", Description = "No specific stool quality assigned (invalid for evaluation).")]
            None = 0,

            /// <summary>
            /// Very hard, dry pellets, crumbly, no residue when picked up.
            /// </summary>
            [Display(Name = "Very Hard", Description = "Very hard, dry pellets, crumbly, no residue when picked up.")]
            VeryHard = 1,

            /// <summary>
            /// Firm, well-formed, log-shaped, segmented, leaves little to no residue.
            /// </summary>
            [Display(Name = "Firm", Description = "Firm, well-formed, log-shaped, segmented, leaves little to no residue.")]
            Firm = 2,

            /// <summary>
            /// Soft, moist, holds shape but leaves slight residue.
            /// </summary>
            [Display(Name = "Soft Formed", Description = "Soft, moist, holds shape but leaves slight residue.")]
            SoftFormed = 3,

            /// <summary>
            /// Soft, pasty, pile-like, unformed, leaves residue.
            /// </summary>
            [Display(Name = "Soft Unformed", Description = "Soft, pasty, pile-like, unformed, leaves residue.")]
            SoftUnformed = 4,

            /// <summary>
            /// Very soft, pudding-like, partially formed, spreads easily.
            /// </summary>
            [Display(Name = "Very Soft", Description = "Very soft, pudding-like, partially formed, spreads easily.")]
            VerySoft = 5,

            /// <summary>
            /// Watery, liquid, no solid form, splatters.
            /// </summary>
            [Display(Name = "Watery", Description = "Watery, liquid, no solid form, splatters.")]
            Watery = 6,

            /// <summary>
            /// Explosive, liquid with blood or mucus, no structure.
            /// </summary>
            [Display(Name = "Explosive", Description = "Explosive, liquid with blood or mucus, no structure.")]
            Explosive = 7
        }
    }
}
