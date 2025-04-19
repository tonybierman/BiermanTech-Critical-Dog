using System.ComponentModel.DataAnnotations;

namespace BiermanTech.CriticalDog.Reports.Columns
{
    /// <summary>
    /// Defines the Orthopedic Foundation for Animals (OFA) hip grades for evaluating hip dysplasia in dogs.
    /// </summary>
    public enum OfaHipsGradeEnum
    {
        /// <summary>
        /// No specific hip grade assigned (invalid for OFA evaluation).
        /// </summary>
        [Display(Name = "None", Description = "No specific hip grade assigned (invalid for OFA evaluation).")]
        None = 0,

        /// <summary>
        /// Near-perfect hip conformation with tight joints and no signs of dysplasia.
        /// </summary>
        [Display(Name = "Excellent", Description = "Near-perfect hip conformation with tight joints and no signs of dysplasia.")]
        Excellent = 1,

        /// <summary>
        /// Well-formed hips with good joint fit, slightly less perfect than excellent.
        /// </summary>
        [Display(Name = "Good", Description = "Well-formed hips with good joint fit, slightly less perfect than excellent.")]
        Good = 2,

        /// <summary>
        /// Minor irregularities in hip conformation, but no significant arthritis or dysplasia.
        /// </summary>
        [Display(Name = "Fair", Description = "Minor irregularities in hip conformation, but no significant arthritis or dysplasia.")]
        Fair = 3,

        /// <summary>
        /// Equivocal findings; hips are not clearly dysplastic but not normal, often requiring re-evaluation.
        /// </summary>
        [Display(Name = "Borderline", Description = "Equivocal findings; hips are not clearly dysplastic but not normal, often requiring re-evaluation.")]
        Borderline = 4,

        /// <summary>
        /// Slight hip joint laxity or incongruity, with minimal arthritic changes.
        /// </summary>
        [Display(Name = "Mild Dysplasia", Description = "Slight hip joint laxity or incongruity, with minimal arthritic changes.")]
        MildDysplasia = 5,

        /// <summary>
        /// Noticeable joint laxity, shallow acetabula, and some arthritic changes.
        /// </summary>
        [Display(Name = "Moderate Dysplasia", Description = "Noticeable joint laxity, shallow acetabula, and some arthritic changes.")]
        ModerateDysplasia = 6,

        /// <summary>
        /// Severe joint laxity, malformed hips, and significant arthritis.
        /// </summary>
        [Display(Name = "Severe Dysplasia", Description = "Severe joint laxity, malformed hips, and significant arthritis.")]
        SevereDysplasia = 7
    }
}

