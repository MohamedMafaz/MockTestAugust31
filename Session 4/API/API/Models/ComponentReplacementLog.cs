using System;
using System.Collections.Generic;

namespace API.Models;

public partial class ComponentReplacementLog
{
    public int LogId { get; set; }

    public int WorkOrderId { get; set; }

    public int ComponentId { get; set; }

    public decimal Quantity { get; set; }

    public DateTime ReplacedAt { get; set; }

    public virtual Component Component { get; set; } = null!;

    public virtual WorkOrder WorkOrder { get; set; } = null!;
}
