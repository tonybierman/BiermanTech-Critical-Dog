using System.Collections.Generic;

namespace BiermanTech.CriticalDog.Data
{
    public class MetricType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int UnitId { get; set; }
        public Unit Unit { get; set; }
        public ICollection<ObservationDefinition> ObservationDefinitions { get; set; } = new List<ObservationDefinition>();
        public ICollection<SubjectRecord> SubjectRecords { get; set; } = new List<SubjectRecord>();
    }
}