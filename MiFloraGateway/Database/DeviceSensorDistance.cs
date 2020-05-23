using System;

namespace MiFloraGateway.Database
{
    public class DeviceSensorDistance
    {
        public Device Device { get; set; } = null!;
        public Sensor Sensor { get; set; } = null!;
        public DateTime When { get; set; }

        public int DeviceId { get; set; }
        public int SensorId { get; set; }

        public int? Rssi { get; set; }
    }
}
