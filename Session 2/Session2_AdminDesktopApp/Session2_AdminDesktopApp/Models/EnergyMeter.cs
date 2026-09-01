using System;
using System.Collections.Generic;

namespace Session2_AdminDesktopApp.Models;

public partial class EnergyMeter
{
    public int MeterId { get; set; }

    public string MeterSerialNumber { get; set; } = null!;

    public int FacilityId { get; set; }

    public string LocationZone { get; set; } = null!;

    public decimal MaxVoltageCapacity { get; set; }

    public decimal BaseTariffRate { get; set; }

    public DateOnly InstallationDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsIndustrial { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();

    public virtual ICollection<ConsumptionLog> ConsumptionLogs { get; set; } = new List<ConsumptionLog>();

    public virtual Facility Facility { get; set; } = null!;
}
