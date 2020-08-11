using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace MiFlora.Common
{
    public class DeviceInfo
    {
        public Version Version { get; set; } = null!;
        public string Name { get; set; } = null!;
        public TimeSpan Uptime { get; set; }
        public PhysicalAddress MACAddress { get; set; } = null!;
    }
}
