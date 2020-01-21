using System;

namespace MiFlora.Common
{
    public class DeviceDiscoveryRequest
    {
        public string Name { get; set; }
        public Version Version { get; set; }
    }

    public class DeviceDiscoveryResponse
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public int Port { get; set; }

    }
}
