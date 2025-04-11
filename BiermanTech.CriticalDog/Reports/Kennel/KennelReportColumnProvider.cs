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
                    DisplayName = "Dog",
                    PropertyName = "Dog.Name",
                    ViewModelName = "DogName",
                    IsSortable = true
                }
                ,
                new ReportColumnDefinition
                {
                    DisplayName = "Observation",
                    PropertyName = "MetricType.ObservationDefinition.DefinitionName",
                    ViewModelName = "ObservationName",
                    IsSortable = true
                }
                ,
                new ReportColumnDefinition
                {
                    DisplayName = "Value",
                    PropertyName = "MetricValue",
                    ViewModelName = "MetricValueWithSymbol",
                    IsSortable = true
                }
            };
        }
    }
}
