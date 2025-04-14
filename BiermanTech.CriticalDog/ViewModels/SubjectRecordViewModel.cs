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

        public string? SubjectName => Subject?.Name;
        public string? SubjectTypeName => Subject?.SubjectType?.TypeName;

        public string? SubjectSex => Subject?.Sex == 0 ? "M" : "F";

        public virtual Subject Subject { get; set; } = null!;

        public virtual MetricType? MetricType { get; set; }

        public virtual ObservationDefinition ObservationDefinition { get; set; } = null!;

        public string? UnitSymbol => MetricType?.Unit?.UnitSymbol;

        public string? ObservationName => StringHelper.SplitPascalCase(ObservationDefinition?.DefinitionName);

        public string? MetricValueWithSymbol => $"{MetricValue} {UnitSymbol}";

        public virtual ICollection<MetaTag> MetaTags { get; set; } = new List<MetaTag>();

        public object? TransformedMetricValue
        {
            get
            {
                if (!MetricValue.HasValue)
                {
                    return null;
                }

                try
                {
                    int mval = checked((int)MetricValue.Value);
                    var transformer = MetricValueTransformProviderFactory.GetProvider(ObservationDefinition.ObservationTypeId);
                    object? retval = transformer?.GetTransormedValue(mval);

                    return retval ?? MetricValue;
                }
                catch (OverflowException)
                {
                    return MetricValue;
                }
            }
        }
    }
}
