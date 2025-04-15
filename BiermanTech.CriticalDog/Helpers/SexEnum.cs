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
        Male = 1,

        /// <summary>
        /// Female
        /// </summary>
        [Display(Name = "Female")]
        Female = 2
    }
}
