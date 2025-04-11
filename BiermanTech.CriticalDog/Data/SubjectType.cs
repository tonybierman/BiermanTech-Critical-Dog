using System;
using System.Collections.Generic;

namespace BiermanTech.CriticalDog.Data;

public partial class SubjectType
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
