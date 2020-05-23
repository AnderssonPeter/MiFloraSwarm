using System;

namespace MiFlora.Common
{
    public class DeviceDiscoveryRequest
    {
        public string Name { get; set; } = null!;
        public Version Version { get; set; } = null!;
    }

    public class DeviceDiscoveryResponse
    {
        public string Name { get; set; } = null!;
        public Version Version { get; set; } = null!;
        public int Port { get; set; }

    }
}
