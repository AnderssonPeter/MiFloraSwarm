using System;
using System.Collections.Generic;
using System.Text;

namespace MiFlora.Common
{
    public class BatteryAndVersionInfo
    {
        public byte Battery { get; set; }
        public Version Version { get; set; } = null!;
        public int Rssi { get; set; }
    }
}
