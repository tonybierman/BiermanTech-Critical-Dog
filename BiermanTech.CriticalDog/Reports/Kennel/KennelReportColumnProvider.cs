using BiermanTech.CriticalDog.Reports.Columns;
using UniversalReportCore;

namespace BiermanTech.CriticalDog.Reports.CityPop
{
    public class KennelReportColumnProvider : IReportColumnProvider
    {
        public string Slug => "Kennel";

        public List<IReportColumnDefinition> GetColumns()
        {
            return new List<IReportColumnDefinition>
            {
                new ReportColumnDefinition
                {
                    IsDisplayKey = true,
                    DisplayName = "Id",
                    PropertyName = "Id",
                    IsSortable = true,
                    DefaultSort = "Asc"
                },
                new ReportColumnDefinition
                {
                    DisplayName = "RecordTime",
                    PropertyName = "RecordTime",
                    IsSortable = true
                }
                ,
                new ReportColumnDefinition
                {
                    DisplayName = "Subject",
                    PropertyName = "Subject.Name",
                    ViewModelName = "SubjectName",
                    IsSortable = true
                }
                ,
                new ReportColumnDefinition
                {
                    DisplayName = "Subject Type",
                    PropertyName = "Subject.SubjectType.TypeName",
                    ViewModelName = "SubjectTypeName",
                    IsSortable = true
                }
                ,
                new ReportColumnDefinition
                {
                    DisplayName = "Sex",
                    PropertyName = "Subject.Sex",
                    ViewModelName = "SubjectSex",
                    IsSortable = true
                }
                ,
                new ReportColumnDefinition
                {
                    DisplayName = "Observation",
                    PropertyName = "ObservationDefinition.DefinitionName",
                    ViewModelName = "ObservationName",
                    IsSortable = true
                }
                ,
                new MetricValueTransformColumn
                {
                    DisplayName = "Value",
                    PropertyName = "MetricValue",
                    ViewModelName = "MetricValue",
                    IsSortable = true
                }
                ,
                new ReportColumnDefinition
                {
                    DisplayName = "Note",
                    PropertyName = "Note",
                    IsSortable = true
                }
            };
        }
    }
}
