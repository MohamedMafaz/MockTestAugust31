using System;
using System.Collections.Generic;

namespace API.Models;

public partial class WorkOrder
{
    public int WorkOrderId { get; set; }

    public int SmartMeterId { get; set; }

    public int TechnicianId { get; set; }

    public int CreatedById { get; set; }

    public string Description { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ComponentReplacementLog> ComponentReplacementLogs { get; set; } = new List<ComponentReplacementLog>();

    public virtual User CreatedBy { get; set; } = null!;

    public virtual SmartMeter SmartMeter { get; set; } = null!;

    public virtual User Technician { get; set; } = null!;
}
