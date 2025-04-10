using System;
using System.Collections.Generic;

namespace BiermanTech.CriticalDog.Data;

public partial class Unit
{
    public int Id { get; set; }

    public string UnitName { get; set; } = null!;

    public string UnitSymbol { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<MetricType> MetricTypes { get; set; } = new List<MetricType>();

    public virtual ICollection<ObservationDefinition> ObservationDefinitions { get; set; } = new List<ObservationDefinition>();
}
