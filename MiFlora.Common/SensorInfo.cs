using System;
using System.Collections.Generic;
using System.Text;

namespace MiFlora.Common
{
    public class SensorInfo
    {
        public string Name { get; set; } = null!;
        public string MACAddress { get; set; } = null!;
        public int Rssi { get; set; }
    }
}
