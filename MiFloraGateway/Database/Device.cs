using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace MiFloraGateway.Database
{
    public class Device
    {
        public int Id { get; set; }
        public string MACAddress { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }
        public ICollection<DeviceSensorDistance> SensorDistances { get; set; }
        public ICollection<DeviceTag> Tags { get; set; }
        public ICollection<LogEntry> Logs { get; set; }

        public override string ToString()
        {
            return $"{Id:000} - {MACAddress} - {Name} - {IPAddress}";
        }
    }
}