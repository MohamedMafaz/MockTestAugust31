using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API.Models;

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
    [JsonIgnore]
    public virtual ICollection<Alert>? Alerts { get; set; } = new List<Alert>();
    [JsonIgnore]
    public virtual ICollection<ConsumptionLog>? ConsumptionLogs { get; set; } = new List<ConsumptionLog>();

    [JsonIgnore]
    public virtual Facility? Facility { get; set; } = null;
}
