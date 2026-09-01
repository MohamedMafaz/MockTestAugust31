using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Facility
{
    public int FacilityId { get; set; }

    public string FacilityName { get; set; } = null!;

    public string LocationZone { get; set; } = null!;

    public string ContactEmail { get; set; } = null!;

    public string GridRegion { get; set; } = null!;

    public decimal MaxCapacityKw { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();

    public virtual ICollection<EnergyMeter> EnergyMeters { get; set; } = new List<EnergyMeter>();
}
