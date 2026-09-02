using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Substation
{
    public int SubstationId { get; set; }

    public string SubstationName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Landmark { get; set; }

    public string Pincode { get; set; } = null!;

    public decimal MaxCapacityKw { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Transformer> Transformers { get; set; } = new List<Transformer>();
}
