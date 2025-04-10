using BiermanTech.CriticalDog.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BiermanTech.CriticalDog.Models
{
    public class CreateObservationViewModel
    {
        [Required]
        public int DogId { get; set; }

        public string DogName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select an observation type.")]
        public int? ObservationDefinitionId { get; set; }

        public int? MetricTypeId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Metric value must be a positive number.")]
        public decimal? MetricValue { get; set; }

        public string? Note { get; set; }

        public DateTime? RecordTime { get; set; } = DateTime.Now; // Default to current time

        public List<int> SelectedMetaTagIds { get; set; } = new List<int>();

        public bool IsQualitative { get; set; }

        public SelectList ObservationDefinitions { get; set; } = null!;
        public SelectList MetricTypes { get; set; } = null!;
        public SelectList MetaTags { get; set; } = null!;
    }
}