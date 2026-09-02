using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class GridContext : DbContext
{
    public GridContext()
    {
    }

    public GridContext(DbContextOptions<GridContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alert> Alerts { get; set; }

    public virtual DbSet<Component> Components { get; set; }

    public virtual DbSet<ComponentReplacementLog> ComponentReplacementLogs { get; set; }

    public virtual DbSet<EnergyLog> EnergyLogs { get; set; }

    public virtual DbSet<IncidentReport> IncidentReports { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationType> NotificationTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SmartMeter> SmartMeters { get; set; }

    public virtual DbSet<Substation> Substations { get; set; }

    public virtual DbSet<TariffPlan> TariffPlans { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    public virtual DbSet<Transformer> Transformers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WorkOrder> WorkOrders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Initial Catalog=GridPulseEnergyDB;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.AlertId).HasName("PK__Alerts__EBB16A8DAE0B208B");

            entity.HasIndex(e => new { e.Status, e.Severity }, "IX_Alerts_Status_Severity");

            entity.Property(e => e.AlertDescription)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.AlertTitle)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Severity)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.SurgeReadingKw)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("SurgeReadingKW");
            entity.Property(e => e.ThresholdLimitKw)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("ThresholdLimitKW");

            entity.HasOne(d => d.SmartMeter).WithMany(p => p.Alerts)
                .HasForeignKey(d => d.SmartMeterId)
                .HasConstraintName("FK__Alerts__SmartMet__6A30C649");
        });

        modelBuilder.Entity<Component>(entity =>
        {
            entity.HasKey(e => e.ComponentId).HasName("PK__Componen__D79CF04E23666B33");

            entity.Property(e => e.ComponentName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<ComponentReplacementLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Componen__5E54864894E82C32");

            entity.Property(e => e.Quantity).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.ReplacedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Component).WithMany(p => p.ComponentReplacementLogs)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Component__Compo__02084FDA");

            entity.HasOne(d => d.WorkOrder).WithMany(p => p.ComponentReplacementLogs)
                .HasForeignKey(d => d.WorkOrderId)
                .HasConstraintName("FK__Component__WorkO__01142BA1");
        });

        modelBuilder.Entity<EnergyLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__EnergyLo__5E548648D3B9AAF9");

            entity.HasIndex(e => new { e.SmartMeterId, e.Timestamp }, "IX_EnergyLogs_SmartMeter_Timestamp").IsDescending(false, true);

            entity.Property(e => e.CurrentAmps).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.PowerKw)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("PowerKW");
            entity.Property(e => e.UnitsKwh)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("UnitsKWh");
            entity.Property(e => e.Voltage).HasColumnType("decimal(6, 2)");

            entity.HasOne(d => d.SmartMeter).WithMany(p => p.EnergyLogs)
                .HasForeignKey(d => d.SmartMeterId)
                .HasConstraintName("FK__EnergyLog__Smart__5AEE82B9");

            entity.HasOne(d => d.TransactionType).WithMany(p => p.EnergyLogs)
                .HasForeignKey(d => d.TransactionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EnergyLog__Trans__5BE2A6F2");
        });

        modelBuilder.Entity<IncidentReport>(entity =>
        {
            entity.HasKey(e => e.IncidentId).HasName("PK__Incident__3D8053B2D0C35EEE");

            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 6)");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Submitted");

            entity.HasOne(d => d.SmartMeter).WithMany(p => p.IncidentReports)
                .HasForeignKey(d => d.SmartMeterId)
                .HasConstraintName("FK__IncidentR__Smart__70DDC3D8");

            entity.HasOne(d => d.User).WithMany(p => p.IncidentReports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__IncidentR__UserI__6FE99F9F");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAB520E3AC05");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.InvoiceUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.SmartMeter).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.SmartMeterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Invoices__SmartM__60A75C0F");

            entity.HasOne(d => d.User).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Invoices__UserId__619B8048");
        });

        modelBuilder.Entity<MaintenanceRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK__Maintena__FBDF78E9A13E31B1");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.SmartMeter).WithMany(p => p.MaintenanceRecords)
                .HasForeignKey(d => d.SmartMeterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Maintenan__Smart__7C4F7684");

            entity.HasOne(d => d.Technician).WithMany(p => p.MaintenanceRecords)
                .HasForeignKey(d => d.TechnicianId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Maintenan__Techn__7D439ABD");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12557929B3");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Message)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.NotificationType).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.NotificationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__Notif__07C12930");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__06CD04F7");
        });

        modelBuilder.Entity<NotificationType>(entity =>
        {
            entity.HasKey(e => e.NotificationTypeId).HasName("PK__Notifica__299002C1810A717D");

            entity.HasIndex(e => e.TypeName, "UQ__Notifica__D4E7DFA834F36C61").IsUnique();

            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AA1C15358");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B616079A55421").IsUnique();

            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SmartMeter>(entity =>
        {
            entity.HasKey(e => e.MeterId).HasName("PK__SmartMet__59223BACFAD34BDF");

            entity.HasIndex(e => e.UserId, "IX_SmartMeters_UserId");

            entity.HasIndex(e => e.MeterSerialNumber, "UQ__SmartMet__36F827E8C7FA452C").IsUnique();

            entity.Property(e => e.DailyUsageLimitKw)
                .HasDefaultValue(50.00m)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("DailyUsageLimitKW");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 6)");
            entity.Property(e => e.MaxVoltageCapacity).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.MeterSerialNumber)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.AssignedTechnician).WithMany(p => p.SmartMeterAssignedTechnicians)
                .HasForeignKey(d => d.AssignedTechnicianId)
                .HasConstraintName("FK__SmartMete__Assig__571DF1D5");

            entity.HasOne(d => d.TariffPlan).WithMany(p => p.SmartMeters)
                .HasForeignKey(d => d.TariffPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SmartMete__Tarif__5812160E");

            entity.HasOne(d => d.Transformer).WithMany(p => p.SmartMeters)
                .HasForeignKey(d => d.TransformerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SmartMete__Trans__5535A963");

            entity.HasOne(d => d.User).WithMany(p => p.SmartMeterUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SmartMete__UserI__5629CD9C");
        });

        modelBuilder.Entity<Substation>(entity =>
        {
            entity.HasKey(e => e.SubstationId).HasName("PK__Substati__BB479C4FB9832793");

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Landmark)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MaxCapacityKw)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("MaxCapacityKW");
            entity.Property(e => e.Pincode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SubstationName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TariffPlan>(entity =>
        {
            entity.HasKey(e => e.TariffPlanId).HasName("PK__TariffPl__29A9280AB6030C10");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PeakHourPricePerUnit).HasColumnType("decimal(8, 4)");
            entity.Property(e => e.PlanName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PricePerUnit).HasColumnType("decimal(8, 4)");
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.HasKey(e => e.TransactionTypeId).HasName("PK__Transact__20266D0B4A956630");

            entity.HasIndex(e => e.TypeName, "UQ__Transact__D4E7DFA8F95C41F1").IsUnique();

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Transformer>(entity =>
        {
            entity.HasKey(e => e.TransformerId).HasName("PK__Transfor__BF26E48FE9A59C9E");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.KiloWattCapacity).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 6)");

            entity.HasOne(d => d.Substation).WithMany(p => p.Transformers)
                .HasForeignKey(d => d.SubstationId)
                .HasConstraintName("FK__Transform__Subst__4BAC3F29");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C939A3B48");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E48E4143F4").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534027B4A3A").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__45F365D3");
        });

        modelBuilder.Entity<WorkOrder>(entity =>
        {
            entity.HasKey(e => e.WorkOrderId).HasName("PK__WorkOrde__AE7551153472EE0A");

            entity.HasIndex(e => e.TechnicianId, "IX_WorkOrders_TechnicianId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Assigned");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.WorkOrderCreatedBies)
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkOrder__Creat__787EE5A0");

            entity.HasOne(d => d.SmartMeter).WithMany(p => p.WorkOrders)
                .HasForeignKey(d => d.SmartMeterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkOrder__Smart__76969D2E");

            entity.HasOne(d => d.Technician).WithMany(p => p.WorkOrderTechnicians)
                .HasForeignKey(d => d.TechnicianId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkOrder__Techn__778AC167");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
