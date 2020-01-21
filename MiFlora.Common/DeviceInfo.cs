using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace MiFlora.Common
{
    public class DeviceInfo
    {
        public Version Version { get; set; }
        public string Name { get; set; }
        public TimeSpan Uptime { get; set; }
        public PhysicalAddress MACAddress { get; set; }
    }
}
