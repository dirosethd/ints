using ints.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ints.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public virtual DbSet<User> Users { get; set; } = null!;
        public DbSet<Car> Cars => Set<Car>();
        public DbSet<Driver> Drivers => Set<Driver>();
        public DbSet<FuelType> FuelTypes => Set<FuelType>();
        public DbSet<Shipment> Shipments => Set<Shipment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasKey(x => x.Id);

                e.HasIndex(x => x.Username).IsUnique();
                e.Property(x => x.Username).HasMaxLength(50).IsRequired();
                e.Property(x => x.PasswordHash).HasColumnType("varbinary(32)").IsRequired();
                e.Property(x => x.PasswordSalt).HasColumnType("varbinary(16)").IsRequired();
                e.Property(x => x.CreatedAtUtc)
                    .HasColumnType("datetime2(7)")
                    .HasDefaultValueSql("SYSUTCDATETIME()");
            });
            modelBuilder.Entity<Car>()
                .HasIndex(x => x.RegNumber)
                .IsUnique();

            modelBuilder.Entity<Car>()
                .HasMany(x => x.Drivers)
                .WithOne(x => x.Car)
                .HasForeignKey(x => x.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Car)
                .WithMany(c => c.Shipments)
                .HasForeignKey(s => s.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Driver)
                .WithMany(d => d.Shipments)
                .HasForeignKey(s => s.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.FuelType)
                .WithMany(f => f.Shipments)
                .HasForeignKey(s => s.FuelTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}