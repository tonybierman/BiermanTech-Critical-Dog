using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Helpers;
using UniversalReportCore.ViewModels;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class SubjectRecordViewModel : IEntityViewModel<int>
    {
        public int Id { get; set; }

        public int SubjectId { get; set; }

        public int? MetricTypeId { get; set; }

        public decimal? MetricValue { get; set; }

        public string? Note { get; set; }

        public DateTime RecordTime { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string? SubjectName => Subject?.Name;
        public string? SubjectTypeName => Subject?.SubjectType?.Name;

        public string? SubjectSex => Subject?.Sex == null ? SexEnum.None.ToString() :  ((SexEnum)Subject?.Sex).ToString();

        public virtual Subject Subject { get; set; } = null!;

        public virtual MetricType? MetricType { get; set; }

        public virtual ObservationDefinition ObservationDefinition { get; set; } = null!;

        public string? UnitSymbol => MetricType?.Unit?.UnitSymbol;

        public string? ObservationName => StringHelper.SplitPascalCase(ObservationDefinition?.Name);

        public string? MetricValueWithSymbol => $"{MetricValue} {UnitSymbol}";

        public virtual ICollection<MetaTag> MetaTags { get; set; } = new List<MetaTag>();
    }
}
