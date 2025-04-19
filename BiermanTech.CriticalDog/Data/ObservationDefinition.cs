using System;
using System.Collections.Generic;

namespace BiermanTech.CriticalDog.Data;

public partial class ObservationDefinition
{
    public int Id { get; set; }

    public string DefinitionName { get; set; } = null!;

    public int ObservationTypeId { get; set; }

    public decimal? MinimumValue { get; set; }

    public decimal? MaximumValue { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }
    public bool? IsSingular { get; set; }

    public virtual ICollection<MetricType> MetricTypes { get; set; } = new List<MetricType>();

    public virtual ObservationType ObservationType { get; set; } = null!;

    public virtual ICollection<SubjectRecord> SubjectRecords { get; set; } = new List<SubjectRecord>();

    public virtual ICollection<ScientificDiscipline> ScientificDisciplines { get; set; } = new List<ScientificDiscipline>();

    public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();
}
