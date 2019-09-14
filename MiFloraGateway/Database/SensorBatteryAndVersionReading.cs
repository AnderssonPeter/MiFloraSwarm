using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiFloraGateway.Database
{
    public class SensorBatteryAndVersionReading
    {
        public int SensorId { get; set; }
        public Sensor Sensor { get; set; }
        public DateTime When { get; set; }
        public int Battery { get; set; }
        public Version Version { get; set; }
    }
}
