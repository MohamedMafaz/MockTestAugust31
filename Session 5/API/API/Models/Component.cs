using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Component
{
    public int ComponentId { get; set; }

    public string ComponentName { get; set; } = null!;

    public int LifetimeInDays { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<ComponentReplacementLog> ComponentReplacementLogs { get; set; } = new List<ComponentReplacementLog>();
}
