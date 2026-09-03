using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Transformer
{
    public int TransformerId { get; set; }

    public int SubstationId { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public decimal KiloWattCapacity { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<SmartMeter> SmartMeters { get; set; } = new List<SmartMeter>();

    public virtual Substation Substation { get; set; } = null!;
}
