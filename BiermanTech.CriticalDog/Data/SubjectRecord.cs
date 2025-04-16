using System;
using System.Collections.Generic;

namespace BiermanTech.CriticalDog.Data;

public partial class SubjectRecord
{
    public int Id { get; set; }

    public int SubjectId { get; set; }

    public int ObservationDefinitionId { get; set; }

    public int? MetricTypeId { get; set; }

    public decimal? MetricValue { get; set; }

    public string? Note { get; set; }

    public DateTime RecordTime { get; set; }

    public virtual MetricType? MetricType { get; set; }

    public virtual ObservationDefinition ObservationDefinition { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual ICollection<MetaTag> MetaTags { get; set; } = new List<MetaTag>();

    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
