using System.ComponentModel.DataAnnotations;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class UnitInputViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UnitName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string UnitSymbol { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool? IsActive { get; set; }
    }
}