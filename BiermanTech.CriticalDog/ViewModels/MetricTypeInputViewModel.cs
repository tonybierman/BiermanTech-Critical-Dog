using System.Collections.Generic;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class MetricTypeInputViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int UnitId { get; set; }
        public IEnumerable<int> ObservationDefinitionIds { get; set; } = new List<int>();
    }
}