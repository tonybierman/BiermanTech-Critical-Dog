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

// Permission flags for bitwise operations
public static class SubjectPermissions
{
    // Anonymous permissions
    public const int AnonymousCanView = 1 << 0; // 1

    // Authenticated permissions
    public const int AuthenticatedCanView = 1 << 1; // 2
    public const int AuthenticatedCanEdit = 1 << 2; // 4

    // Owner permissions
    public const int OwnerCanView = 1 << 3; // 8
    public const int OwnerCanEdit = 1 << 4; // 16
    public const int OwnerCanDelete = 1 << 5; // 32

    // Admin permissions (immutable)
    public const int AdminCanView = 1 << 6; // 64
    public const int AdminCanEdit = 1 << 7; // 128
    public const int AdminCanDelete = 1 << 8; // 256
}