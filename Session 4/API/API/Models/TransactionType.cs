using System;
using System.Collections.Generic;

namespace API.Models;

public partial class TransactionType
{
    public int TransactionTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<EnergyLog> EnergyLogs { get; set; } = new List<EnergyLog>();
}
