using System;
using System.Collections.Generic;

namespace BiermanTech.CriticalDog.Data;

public partial class SubjectType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Clade { get; set; } = null!;
    public string ScientificName { get; set; } = null!;
    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
