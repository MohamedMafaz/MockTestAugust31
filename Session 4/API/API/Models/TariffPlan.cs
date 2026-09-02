using System;
using System.Collections.Generic;

namespace API.Models;

public partial class TariffPlan
{
    public int TariffPlanId { get; set; }

    public string PlanName { get; set; } = null!;

    public decimal PricePerUnit { get; set; }

    public decimal PeakHourPricePerUnit { get; set; }

    public int MaximumUnits { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<SmartMeter> SmartMeters { get; set; } = new List<SmartMeter>();
}
