using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiermanTech.CriticalDog.Data;

public partial class Subject
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Breed { get; set; }

    public sbyte Sex { get; set; }

    public DateOnly? ArrivalDate { get; set; }

    public string? Notes { get; set; }

    public int? SubjectTypeId { get; set; }

    // Permissions field for bitwise operations
    public int Permissions { get; set; }

    public virtual ICollection<SubjectRecord> SubjectRecords { get; set; } = new List<SubjectRecord>();

    public virtual ICollection<MetaTag> MetaTags { get; set; } = new List<MetaTag>();

    public virtual SubjectType? SubjectType { get; set; }

    // Ownership: Foreign key to IdentityUser
    public string UserId { get; set; }

    // Optional: Navigation property to IdentityUser
    [ForeignKey("UserId")]
    public IdentityUser User { get; set; }

    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

