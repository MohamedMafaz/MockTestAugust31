using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class EnergyContext : DbContext
{
    public EnergyContext()
    {
    }

    public EnergyContext(DbContextOptions<EnergyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alert> Alerts { get; set; }

    public virtual DbSet<ConsumptionLog> ConsumptionLogs { get; set; }

    public virtual DbSet<EnergyMeter> EnergyMeters { get; set; }

    public virtual DbSet<Facility> Facilities { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Initial Catalog=EnergyManagementDB;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.AlertId).HasName("PK__Alerts__EBB16AEDB23F32BE");

            entity.Property(e => e.AlertId).HasColumnName("AlertID");
            entity.Property(e => e.EventTimestamp).HasColumnType("datetime");
            entity.Property(e => e.FacilityId).HasColumnName("FacilityID");
            entity.Property(e => e.MeterId).HasColumnName("MeterID");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Severity)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.SurgeReadingKw)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("SurgeReadingKW");
            entity.Property(e => e.ThresholdLimitKw)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("ThresholdLimitKW");

            entity.HasOne(d => d.Facility).WithMany(p => p.Alerts)
                .HasForeignKey(d => d.FacilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Alerts__Facility__45F365D3");

            entity.HasOne(d => d.Meter).WithMany(p => p.Alerts)
                .HasForeignKey(d => d.MeterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Alerts__MeterID__46E78A0C");
        });

        modelBuilder.Entity<ConsumptionLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Consumpt__5E5499A86942C806");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.CurrentAmps).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.MeterId).HasColumnName("MeterID");
            entity.Property(e => e.PowerKw)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("PowerKW");
            entity.Property(e => e.Timestamp).HasColumnType("datetime");
            entity.Property(e => e.Voltage).HasColumnType("decimal(6, 2)");

            entity.HasOne(d => d.Meter).WithMany(p => p.ConsumptionLogs)
                .HasForeignKey(d => d.MeterId)
                .HasConstraintName("FK__Consumpti__Meter__403A8C7D");
        });

        modelBuilder.Entity<EnergyMeter>(entity =>
        {
            entity.HasKey(e => e.MeterId).HasName("PK__EnergyMe__59223B8C215FBD7A");

            entity.HasIndex(e => e.MeterSerialNumber, "UQ__EnergyMe__36F827E83C94BAF0").IsUnique();

            entity.Property(e => e.MeterId).HasColumnName("MeterID");
            entity.Property(e => e.BaseTariffRate).HasColumnType("decimal(6, 4)");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FacilityId).HasColumnName("FacilityID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LocationZone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaxVoltageCapacity).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.MeterSerialNumber)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Facility).WithMany(p => p.EnergyMeters)
                .HasForeignKey(d => d.FacilityId)
                .HasConstraintName("FK__EnergyMet__Facil__3D5E1FD2");
        });

        modelBuilder.Entity<Facility>(entity =>
        {
            entity.HasKey(e => e.FacilityId).HasName("PK__Faciliti__5FB08B948055FB39");

            entity.Property(e => e.FacilityId).HasColumnName("FacilityID");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FacilityName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.GridRegion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LocationZone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaxCapacityKw)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("MaxCapacityKW");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC079621EB5D");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0758149482");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__4CA06362");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
