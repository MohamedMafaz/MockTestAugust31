using System;
using System.Collections.Generic;

namespace API.Models;

public partial class SmartMeter
{
    public int MeterId { get; set; }

    public string MeterSerialNumber { get; set; } = null!;

    public int TransformerId { get; set; }

    public int UserId { get; set; }

    public int? AssignedTechnicianId { get; set; }

    public int TariffPlanId { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public decimal MaxVoltageCapacity { get; set; }

    public decimal DailyUsageLimitKw { get; set; }

    public DateOnly InstallationDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsIndustrial { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();

    public virtual User? AssignedTechnician { get; set; }

    public virtual ICollection<EnergyLog> EnergyLogs { get; set; } = new List<EnergyLog>();

    public virtual ICollection<IncidentReport> IncidentReports { get; set; } = new List<IncidentReport>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();

    public virtual TariffPlan TariffPlan { get; set; } = null!;

    public virtual Transformer Transformer { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
}
