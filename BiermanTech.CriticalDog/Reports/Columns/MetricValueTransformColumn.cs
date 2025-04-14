using BiermanTech.CriticalDog.ViewModels;
using UniversalReportCore;

namespace BiermanTech.CriticalDog.Reports.Columns
{
    public class MetricValueTransformColumn : ReportColumnDefinition
    {
        public MetricValueTransformColumn() 
        {
            RenderPartial = "_MetricValueTransformFieldPartial";
            ViewModelType = typeof(MetricValueTransformFieldViewModel);
        }
    }
}
