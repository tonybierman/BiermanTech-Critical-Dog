using BiermanTech.CriticalDog.Data;
using UniversalReportCore.ViewModels;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class DogRecordViewModel : IEntityViewModel<int>
    {
        public int Id { get; set; }

        public int DogId { get; set; }

        public int? MetricTypeId { get; set; }

        public decimal? MetricValue { get; set; }

        public string? Note { get; set; }

        public DateTime RecordTime { get; set; }

        public string? CreatedBy { get; set; }

        public string? DogName => Dog?.Name;

        public virtual Dog Dog { get; set; } = null!;

        public virtual MetricType? MetricType { get; set; }

        public string? UnitSymbol => MetricType?.Unit?.UnitSymbol;

        public string? ObservationName => MetricType?.ObservationDefinition?.DefinitionName;

        public string? MetricValueWithSymbol => $"{MetricValue} {UnitSymbol}";

        public virtual ICollection<MetaTag> MetaTags { get; set; } = new List<MetaTag>();
    }
}
