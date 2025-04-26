using BiermanTech.CriticalDog.ViewModels;
using UniversalReportCore;

namespace BiermanTech.CriticalDog.Reports.Columns
{
    public class EntityToolbarColumn : ReportColumnDefinition
    {
        public string? EntityBaseRoute { get; set; }
        public string? SecurityPolicy { get; set; }

        public EntityToolbarColumn() 
        {
            RenderPartial = "_ColumnEntityToolbarPartial";
        }
    }
}
