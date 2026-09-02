using System;
using System.Collections.Generic;

namespace API.Models;

public partial class MaintenanceRecord
{
    public int RecordId { get; set; }

    public int SmartMeterId { get; set; }

    public int TechnicianId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual SmartMeter SmartMeter { get; set; } = null!;

    public virtual User Technician { get; set; } = null!;
}
