using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Models
{
    public class CreateObservationViewModel
    {
        public int DogId { get; set; }
        public string? DogName { get; set; }
        public int? ObservationDefinitionId { get; set; }
        public int? MetricTypeId { get; set; }
        public decimal? MetricValue { get; set; }
        public string? Note { get; set; }
        public DateTime? RecordTime { get; set; }
        public bool IsQualitative { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public List<int> SelectedMetaTagIds { get; set; } = new List<int>();
        public SelectList ObservationDefinitions { get; set; }
        public SelectList MetricTypes { get; set; }
        public SelectList MetaTags { get; set; }
    }
}