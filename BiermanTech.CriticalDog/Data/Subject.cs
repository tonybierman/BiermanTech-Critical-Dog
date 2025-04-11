using System;
using System.Collections.Generic;

namespace BiermanTech.CriticalDog.Data;

public partial class Subject
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Breed { get; set; }

    public int? Age { get; set; }

    public sbyte Sex { get; set; }

    public decimal? WeightKg { get; set; }

    public DateOnly? ArrivalDate { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<SubjectRecord> SubjectRecords { get; set; } = new List<SubjectRecord>();
}
