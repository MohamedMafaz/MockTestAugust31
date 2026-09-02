using System;
using System.Collections.Generic;

namespace API.Models;

public partial class EnergyLog
{
    public long LogId { get; set; }

    public int SmartMeterId { get; set; }

    public DateTime Timestamp { get; set; }

    public decimal UnitsKwh { get; set; }

    public decimal Voltage { get; set; }

    public decimal CurrentAmps { get; set; }

    public decimal PowerKw { get; set; }

    public bool IsPeakHour { get; set; }

    public int TransactionTypeId { get; set; }

    public virtual SmartMeter SmartMeter { get; set; } = null!;

    public virtual TransactionType TransactionType { get; set; } = null!;
}
