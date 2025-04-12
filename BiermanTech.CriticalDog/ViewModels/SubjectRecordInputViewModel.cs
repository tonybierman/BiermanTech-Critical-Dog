using System.ComponentModel.DataAnnotations;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class SubjectRecordInputViewModel
    {
        public int Id { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public int ObservationDefinitionId { get; set; }

        public int? MetricTypeId { get; set; }

        public decimal? MetricValue { get; set; }

        [StringLength(1000)]
        public string? Note { get; set; }

        [Required]
        public DateTime RecordTime { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public List<int> SelectedMetaTagIds { get; set; } = new List<int>();
    }
}