using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ints.Models;

public partial class IntsContext : DbContext
{
    public IntsContext()
    {
    }

    public IntsContext(DbContextOptions<IntsContext> options) 
        : base(options)
    {
    }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<FuelType> FuelTypes { get; set; }

    public virtual DbSet<Shipment> Shipments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DIROSE;Initial Catalog=ints;Persist Security Info=True;User ID=dirosethd;Password=1612;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cars__3214EC07A2823550");

            entity.Property(e => e.Brand).HasMaxLength(50);
            entity.Property(e => e.Model).HasMaxLength(50);
            entity.Property(e => e.RegNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Drivers__3214EC075E509C46");

            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.LicenseNumber).HasMaxLength(20);

            entity.HasOne(d => d.Car).WithMany(p => p.Drivers)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Drivers__CarId__4E88ABD4");
        });

        modelBuilder.Entity<FuelType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FuelType__3214EC079064F516");

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PricePerLiter).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shipment__3214EC073DB236F5");

            entity.Property(e => e.DistanceKm).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FromLocation).HasMaxLength(200);
            entity.Property(e => e.ToLocation).HasMaxLength(200);
            entity.Property(e => e.VolumeLiters).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Car).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Shipments__CarId__534D60F1");

            entity.HasOne(d => d.Driver).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Shipments__Drive__5441852A");

            entity.HasOne(d => d.FuelType).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.FuelTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Shipments__FuelT__5535A963");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username, "UX_Users_Username").IsUnique();

            entity.Property(e => e.CreatedAtUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.PasswordHash).HasMaxLength(32);
            entity.Property(e => e.PasswordSalt).HasMaxLength(16);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
