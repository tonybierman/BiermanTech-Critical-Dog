using System;
using System.Collections.Generic;

namespace BiermanTech.CriticalDog.Data;

public partial class DogRecord
{
    public int Id { get; set; }

    public int DogId { get; set; }

    public int? MetricTypeId { get; set; }

    public decimal? MetricValue { get; set; }

    public string? Note { get; set; }

    public DateTime RecordTime { get; set; }

    public string? CreatedBy { get; set; }

    public virtual Dog Dog { get; set; } = null!;

    public virtual MetricType? MetricType { get; set; }

    public virtual ICollection<MetaTag> MetaTags { get; set; } = new List<MetaTag>();
}
