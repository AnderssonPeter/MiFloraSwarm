using System.Collections.Generic;

namespace MiFloraGateway.Database
{
    public class Device
    {
        public int Id { get; set; }
        public string MACAddress { get; set; } = null!;
        public string IPAddress { get; set; } = null!;
        public int Port { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<DeviceSensorDistance> SensorDistances { get; set; } = null!;
        public ICollection<DeviceTag> Tags { get; set; } = null!;
        public ICollection<LogEntry> Logs { get; set; } = null!;

        public override string ToString()
        {
            return $"{Id:000} - {MACAddress} - {Name} - {IPAddress}";
        }
    }
}