using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiermanTech.CriticalDog.Data;

public partial class MetaTag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public string? UserId { get; set; }
    // Optional: Navigation property to IdentityUser
    [ForeignKey("UserId")]
    public IdentityUser? User { get; set; }
    public virtual ICollection<SubjectRecord> SubjectRecords { get; set; } = new List<SubjectRecord>();
    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
