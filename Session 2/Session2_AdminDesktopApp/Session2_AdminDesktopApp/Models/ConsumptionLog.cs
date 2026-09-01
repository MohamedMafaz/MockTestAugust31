using System;
using System.Collections.Generic;

namespace Session2_AdminDesktopApp.Models;

public partial class ConsumptionLog
{
    public long LogId { get; set; }

    public int MeterId { get; set; }

    public DateTime Timestamp { get; set; }

    public decimal Voltage { get; set; }

    public decimal CurrentAmps { get; set; }

    public decimal PowerKw { get; set; }

    public bool PeakHour { get; set; }

    public virtual EnergyMeter Meter { get; set; } = null!;
}
