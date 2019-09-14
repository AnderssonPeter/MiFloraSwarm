using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiFloraGateway.Database
{
    public class DeviceSensorDistance
    {
        public Device Device { get; set; }
        public Sensor Sensor { get; set; }
        public DateTime When { get; set; }

        public int DeviceId { get; set; }
        public int SensorId { get; set; }

        public int Rssi { get; set; }
    }
}
