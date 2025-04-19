using System.ComponentModel.DataAnnotations;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class ObservationDefinitionInputViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Definition Name must contain only alphanumeric characters (no spaces or special characters).")] 
        public string DefinitionName { get; set; } = string.Empty;

        [Required]
        public int ObservationTypeId { get; set; }

        public decimal? MinimumValue { get; set; }

        public decimal? MaximumValue { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsSingular { get; set; }

        public List<int> SelectedScientificDisciplineIds { get; set; } = new List<int>();
    }
}