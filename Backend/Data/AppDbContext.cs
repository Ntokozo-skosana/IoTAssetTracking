using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DeviceType> DeviceType { get; set; }
        public DbSet<DeviceGroup> DeviceGroup { get; set; }
        public DbSet<Firmware> Firmware { get; set; }
        public DbSet<Device> Device { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<DeviceGroup>()
                .HasOne(g => g.ParentGroup)
                .WithMany(g => g.ChildGroups)
                .HasForeignKey(g => g.ParentGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Device>()
                .HasOne(d => d.DeviceType)
                .WithMany(dt => dt.Devices)
                .HasForeignKey(d => d.DeviceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Device>()
                .HasOne(d => d.Firmware)
                .WithMany(f => f.Devices)
                .HasForeignKey(d => d.FirmwareId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Device>()
                .HasOne(d => d.Group)
                .WithMany(g => g.Devices)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Firmware>()
                .HasOne(f => f.DeviceType)
                .WithMany(dt => dt.Firmwares)
                .HasForeignKey(f => f.DeviceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Device>()
                .HasIndex(d => d.SerialNumber)
                .IsUnique();
        }
    }
}