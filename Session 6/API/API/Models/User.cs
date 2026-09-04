using System;
using System.Collections.Generic;

namespace API.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public int RoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<IncidentReport> IncidentReports { get; set; } = new List<IncidentReport>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<SmartMeter> SmartMeterAssignedTechnicians { get; set; } = new List<SmartMeter>();

    public virtual ICollection<SmartMeter> SmartMeterUsers { get; set; } = new List<SmartMeter>();

    public virtual ICollection<WorkOrder> WorkOrderCreatedBies { get; set; } = new List<WorkOrder>();

    public virtual ICollection<WorkOrder> WorkOrderTechnicians { get; set; } = new List<WorkOrder>();
}
