using System.ComponentModel.DataAnnotations;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class MetricTypeInputViewModel
    {
        public int Id { get; set; }

        [Required]
        public int ObservationDefinitionId { get; set; }

        [Required]
        public int UnitId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool? IsActive { get; set; }
    }
}