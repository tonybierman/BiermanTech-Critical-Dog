using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Helpers;
using System.Security.Cryptography.Xml;
using UniversalReportCore;
using UniversalReportCore.Ui.ViewModels;
using UniversalReportCore.ViewModels;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class MetricValueTransformFieldViewModel : EntityFieldViewModel
    {
        private readonly SubjectRecordViewModel _parent;
        private readonly IReportColumnDefinition _column;
        private readonly string? _slug;

        public string? TransformedValue 
        {
            get
            {
                if (!_parent.MetricValue.HasValue || _parent.ObservationDefinition == null)
                {
                    return null;
                }

                try
                {
                    int mval = checked((int)_parent.MetricValue.Value);
                    var transformer = MetricValueTransformerFactory.GetProvider(_parent.ObservationDefinition);
                    string? retval = transformer?.GetTransformedValue(mval);

                    return retval ?? $"{_parent.MetricValue}";
                }
                catch (NullReferenceException)
                {
                    return $"{_parent.MetricValue}";
                }
                catch (OverflowException)
                {
                    return $"{_parent.MetricValue}";
                }
            }
        }
        public MetricValueTransformFieldViewModel(SubjectRecordViewModel parent, IReportColumnDefinition column, string? slug) : base(parent, column, slug)
        {
            _parent = parent;
            _column = column;
            _slug = slug;
        }

    }
}
