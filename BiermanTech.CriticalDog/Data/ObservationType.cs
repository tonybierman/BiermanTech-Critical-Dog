using System;
using System.Collections.Generic;

namespace BiermanTech.CriticalDog.Data;

public partial class ObservationType
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<ObservationDefinition> ObservationDefinitions { get; set; } = new List<ObservationDefinition>();
}
