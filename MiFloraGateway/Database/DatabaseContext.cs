using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MiFloraGateway.Database
{
    public class DatabaseContext : IdentityDbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceTag> DevicesTags { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<DeviceSensorDistance> DeviceSensorDistances { get; set; }
        public DbSet<SensorDataReading> SensorDataReadings { get; set; }
        public DbSet<SensorBatteryAndVersionReading> SensorBatteryReadings { get; set; }
        public DbSet<SensorTag> SensorTags { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<PlantBasic> PlantBasics { get; set; }
        public DbSet<PlantMaintenance> PlantMaintenance { get; set; }
        public DbSet<PlantParameters> PlantParameters { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Device>().HasKey(s => s.Id);
            builder.Entity<Device>().Property(d => d.Id).UseIdentityColumn();

            builder.Entity<Device>().Property(d => d.MACAddress).IsRequired().HasMaxLength(17).IsFixedLength();
            builder.Entity<Device>().HasIndex(d => d.MACAddress).IsUnique();

            builder.Entity<Device>().Property(d => d.IPAddress).IsRequired().HasMaxLength(45);
            builder.Entity<Device>().HasIndex(d => d.IPAddress).IsUnique();

            builder.Entity<Device>().HasIndex(d => d.Name).IsUnique();

            builder.Entity<Device>().HasMany(d => d.SensorDistances).WithOne(dsd => dsd.Device).HasForeignKey(dsd => dsd.DeviceId);
            builder.Entity<Device>().HasMany(d => d.Tags).WithOne(dt => dt.Device).HasForeignKey(dt => dt.DeviceId);
            builder.Entity<Device>().HasMany(d => d.Logs).WithOne(le => le.Device).HasForeignKey(le => le.DeviceId);

            builder.Entity<DeviceTag>().HasKey(dt => new { dt.DeviceId, dt.Tag });
            builder.Entity<DeviceTag>().Property(dt => dt.Tag).HasMaxLength(32);
            builder.Entity<DeviceTag>().Property(dt => dt.Value).IsRequired().HasMaxLength(32);

            builder.Entity<LogEntry>().HasKey(dl => dl.Id);
            builder.Entity<LogEntry>().Property(dl => dl.Id).ValueGeneratedOnAdd();
            builder.Entity<LogEntry>().Property(dl => dl.Event).HasMaxLength(Enum<LogEntryEvent>.GetValues().Max(x => x.ToString().Length))
                                                                .IsRequired()
                                                                .HasConversion(x => x.ToString(), x => Enum<LogEntryEvent>.Parse(x));
            builder.Entity<LogEntry>().Property(dl => dl.Result).HasMaxLength(Enum<LogEntryResult>.GetValues().Max(x => x.ToString().Length))
                                                                .IsRequired()
                                                                .HasConversion(x => x.ToString(), x => Enum<LogEntryResult>.Parse(x));
            builder.Entity<LogEntry>().Property(dl => dl.Message).HasMaxLength(200);

            builder.Entity<Sensor>().HasKey(s => s.Id);
            builder.Entity<Sensor>().Property(s => s.Id).UseIdentityColumn();
            builder.Entity<Sensor>().HasIndex(d => d.MACAddress);
            builder.Entity<Sensor>().Property(s => s.MACAddress).IsRequired().HasMaxLength(17).IsFixedLength();
            builder.Entity<Sensor>().HasMany(s => s.DeviceDistances).WithOne(dsd => dsd.Sensor).HasForeignKey(dsd => dsd.SensorId);
            builder.Entity<Sensor>().HasMany(s => s.BatteryAndVersionReadings).WithOne(sbr => sbr.Sensor).HasForeignKey(sbr => sbr.SensorId);
            builder.Entity<Sensor>().HasMany(s => s.DataReadings).WithOne(sdr => sdr.Sensor).HasForeignKey(sdr => sdr.SensorId);
            builder.Entity<Sensor>().HasMany(s => s.Tags).WithOne(st => st.Sensor).HasForeignKey(st => st.SensorId);
            builder.Entity<Sensor>().HasMany(s => s.Logs).WithOne(le => le.Sensor).HasForeignKey(le => le.SensorId);

            builder.Entity<DeviceSensorDistance>().HasKey(dsd => new { dsd.DeviceId, dsd.SensorId, dsd.When });

            builder.Entity<SensorDataReading>().HasKey(sdr => new { sdr.SensorId, sdr.When });

            builder.Entity<SensorBatteryAndVersionReading>().HasKey(sbr => new { sbr.SensorId, sbr.When });
            builder.Entity<SensorBatteryAndVersionReading>().Property(s => s.Version).HasConversion(v => v.ToString(), s => Version.Parse(s)).HasMaxLength(12).IsRequired();

            builder.Entity<SensorTag>().HasKey(st => new { st.SensorId, st.Tag });
            builder.Entity<SensorTag>().Property(st => st.Tag).HasMaxLength(32);
            builder.Entity<SensorTag>().Property(st => st.Value).IsRequired().HasMaxLength(32);

            builder.Entity<Plant>().HasKey(p => p.Id);
            builder.Entity<Plant>().Property(p => p.Id).UseIdentityColumn();
            builder.Entity<Plant>().HasIndex(p => p.LatinName).IsUnique();
            builder.Entity<Plant>()
                .HasOne(p => p.Basic)
                .WithOne(b => b.Plant)
                .HasForeignKey<PlantBasic>(p => p.PlantId);

            builder.Entity<Plant>()
                .HasOne(p => p.Maintenance)
                .WithOne(m => m.Plant)
                .HasForeignKey<PlantMaintenance>(m => m.PlantId);

            builder.Entity<Plant>()
                .HasOne(p => p.Parameters)
                .WithOne(p => p.Plant)
                .HasForeignKey<PlantParameters>(p => p.PlantId);

            builder.Entity<PlantBasic>().HasKey(b => b.PlantId);

            builder.Entity<PlantParameters>().HasKey(p => p.PlantId);
            builder.Entity<PlantParameters>().OwnsOne(p => p.EnvironmentHumidity);
            builder.Entity<PlantParameters>().OwnsOne(p => p.LightLux);
            builder.Entity<PlantParameters>().OwnsOne(p => p.LightMmol);
            builder.Entity<PlantParameters>().OwnsOne(p => p.SoilFertility);
            builder.Entity<PlantParameters>().OwnsOne(p => p.SoilHumidity);
            builder.Entity<PlantParameters>().OwnsOne(p => p.Temperature);

            builder.Entity<PlantMaintenance>().HasKey(m => m.PlantId);

            builder.Entity<Setting>().HasKey(s => s.Key);

            base.OnModelCreating(builder);
        }

        public LogEntryHandler AddLogEntry(LogEntryEvent @event, Device device = null, Sensor sensor = null, Plant plant = null, IdentityUser user = null)
        {
            return new LogEntryHandler(this, @event, device: device, sensor: sensor, plant: plant, user: user);
        }

    }

    public class Setting
    {
        public Settings Key { get; set; }
        public string Value { get; set; }
        public DateTime? LastChanged { get; set; }
    }

    public class LogEntryHandler : IDisposable
    {
        private readonly DatabaseContext databaseContext;
        private readonly LogEntryEvent @event;
        private readonly Device device;
        private readonly Sensor sensor;
        private readonly Plant plant;
        private readonly IdentityUser user;
        private readonly DateTime when = DateTime.Now;
        private bool saved = false;

        public LogEntryHandler(DatabaseContext databaseContext, LogEntryEvent @event, Device device = null, Sensor sensor = null, Plant plant = null, IdentityUser user = null)
        {
            this.databaseContext = databaseContext;
            this.@event = @event;
            this.device = device;
            this.sensor = sensor;
            this.plant = plant;
            this.user = user;
        }

        public void Success(string message = null) => Save(LogEntryResult.Successful, message);

        public void Failure(string message = null) => Save(LogEntryResult.Failed, message);

        public void Failure(Exception ex, string message = null) => Save(LogEntryResult.Failed, ex.ToString() + Environment.NewLine + message);

        private void Save(LogEntryResult result, string message)
        {
            var logEntry = new LogEntry
            {
                Device = device,
                Sensor = sensor,
                Plant = plant,
                User = user,
                When = when,
                Duration = DateTime.Now.Subtract(when),
                Event = @event,
                Result = result,
                Message = message.Truncate(200)
            };
            databaseContext.LogEntries.Add(logEntry);
        }

        public void Dispose()
        {
        }
    }

}
