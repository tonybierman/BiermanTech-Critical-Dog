using System.Collections.Generic;

namespace BiermanTech.CriticalDog.Data
{
    public class ObservationDefinition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsSingular { get; set; }
        public decimal? MaximumValue { get; set; }
        public decimal? MinimumValue { get; set; }
        public int ObservationTypeId { get; set; }
        public ObservationType ObservationType { get; set; }
        public ICollection<ScientificDiscipline> ScientificDisciplines { get; set; } = new List<ScientificDiscipline>();
        public ICollection<MetricType> MetricTypes { get; set; } = new List<MetricType>();
        public ICollection<Unit> Units { get; set; } = new List<Unit>();
        public ICollection<SubjectRecord> SubjectRecords { get; set; } = new List<SubjectRecord>();
    }
}