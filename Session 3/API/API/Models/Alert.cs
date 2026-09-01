using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Alert
{
    public int AlertId { get; set; }

    public int FacilityId { get; set; }

    public int MeterId { get; set; }

    public DateTime EventTimestamp { get; set; }

    public decimal SurgeReadingKw { get; set; }

    public decimal ThresholdLimitKw { get; set; }

    public string Severity { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual Facility Facility { get; set; } = null!;

    public virtual EnergyMeter Meter { get; set; } = null!;
}
