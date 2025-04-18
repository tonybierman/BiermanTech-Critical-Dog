using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Models
{
    public class CreateObservationViewModel
    {
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public int? ObservationDefinitionId { get; set; }
        public int? ObservationDefinitionTypeName { get; set; }
        public int? MetricTypeId { get; set; }
        public decimal? MetricValue { get; set; }
        public string? Note { get; set; }
        public DateTime? RecordTime { get; set; }
        public decimal? MinimumValue { get; set; }
        public decimal? MaximumValue { get; set; }
        public List<int> SelectedMetaTagIds { get; set; } = new List<int>();
        public SelectList ObservationDefinitions { get; set; }
        public SelectList MetricTypes { get; set; }
        public SelectList MetaTags { get; set; }
    }
}