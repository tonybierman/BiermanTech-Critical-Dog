using System.ComponentModel.DataAnnotations;

namespace BiermanTech.CriticalDog.Helpers
{
    public enum SexEnum
    {
        /// <summary>
        /// No gender rassigned.
        /// </summary>
        [Display(Name = "None")]
        None = 0,

        /// <summary>
        /// Male
        /// </summary>
        [Display(Name = "Male")]
        M = 1,

        /// <summary>
        /// Female
        /// </summary>
        [Display(Name = "Female")]
        F = 2
    }
}
