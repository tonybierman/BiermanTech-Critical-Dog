using System;
using System.Collections.Generic;

namespace BiermanTech.CriticalDog.Data;

public partial class MetricType
{
    public int Id { get; set; }

    public int ObservationDefinitionId { get; set; }

    public int UnitId { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public virtual ObservationDefinition ObservationDefinition { get; set; } = null!;

    public virtual ICollection<SubjectRecord> SubjectRecords { get; set; } = new List<SubjectRecord>();

    public virtual Unit Unit { get; set; } = null!;
}
