using System.Collections.Generic;

namespace MiFloraGateway.Database
{
    public class Sensor
    {
        public int Id { get; set; }
        public string MACAddress { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Plant? Plant { get; set; }
        public ICollection<DeviceSensorDistance> DeviceDistances { get; set; } = null!;
        public ICollection<SensorBatteryAndVersionReading> BatteryAndVersionReadings { get; set; } = null!;
        public ICollection<SensorDataReading> DataReadings { get; set; } = null!;
        public ICollection<SensorTag> Tags { get; set; } = null!;
        public ICollection<LogEntry> Logs { get; set; } = null!;

        public override string ToString()
        {
            return $"{Id:000} - {MACAddress} - {Name}";
        }
    }
}
