using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Alert
{
    public int AlertId { get; set; }

    public int SmartMeterId { get; set; }

    public string AlertTitle { get; set; } = null!;

    public string AlertDescription { get; set; } = null!;

    public decimal? SurgeReadingKw { get; set; }

    public decimal? ThresholdLimitKw { get; set; }

    public string Severity { get; set; } = null!;

    public string Status { get; set; } = null!;

    public bool IsViewed { get; set; }

    public bool IsEmergency { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual SmartMeter SmartMeter { get; set; } = null!;
}
