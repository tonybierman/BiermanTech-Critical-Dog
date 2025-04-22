using System;
using System.Collections.Generic;

namespace BiermanTech.CriticalDog.Data;

public partial class MetaTag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<SubjectRecord> SubjectRecords { get; set; } = new List<SubjectRecord>();
}
