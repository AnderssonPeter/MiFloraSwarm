using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreFlora.Controllers
{
    public enum DeviceSupportFlags
    {
        Shutdown,
        Restart
    }

    public class DeviceInformation
    {
        public Version Version { get; set; }
        public PhysicalAddress PhysicalAddress { get; set; }
        public Runtime Runtime { get; set; }
        public OperatingSystem OperatingSystem { get; set; }
        public Process Process { get; set; }

        public IEnumerable<DeviceSupportFlags> Supports { get; set; }
    }

    public class Runtime
    {
        public Version Version { get; set; }
        public string Description { get; set; }
    }

    public class OperatingSystem
    {
        public bool Is64Bit { get; set; }
        public string Name { get; set; }
        public TimeSpan Uptime { get; set; }
    }

    public class Process
    {
        public bool Is64Bit { get; set; }
        public TimeSpan Uptime { get; set; }

    }

    [ApiController]
    [Route("[controller]")]
    public class DeviceController : ControllerBase
    {

        private readonly ILogger<DeviceController> _logger;

        public DeviceController(ILogger<DeviceController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public DeviceInformation Get()
        {
            return new DeviceInformation()
            {
                Version = Assembly.GetExecutingAssembly().GetName().Version,
                PhysicalAddress = NetworkInterface.GetAllNetworkInterfaces()
                                                  .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                                                  .Select(x => x.GetPhysicalAddress()).FirstOrDefault(),
                Runtime = new Runtime
                {
                    Version = Environment.Version,
                    Description = RuntimeInformation.FrameworkDescription
                },
                OperatingSystem = new OperatingSystem
                {
                    Is64Bit = Environment.Is64BitOperatingSystem,
                    Name = RuntimeInformation.OSDescription,
                    Uptime = GetOSUptime()
                },
                Process = new Process
                {
                    Is64Bit = Environment.Is64BitProcess,
                    Uptime = GetProcessUptime()
                },
                Supports = new[] { DeviceSupportFlags.Shutdown }

            };
        }
        
        [HttpPost("shutdown")]
        public IActionResult Shutdown()
        {
            throw new NotImplementedException();
            return Ok();
        }

        public TimeSpan GetProcessUptime()
        {
            return DateTime.Now.Subtract(System.Diagnostics.Process.GetCurrentProcess().StartTime);
        }

        public TimeSpan GetOSUptime()
        {
            if (Stopwatch.IsHighResolution)
            {
                return TimeSpan.FromSeconds(Stopwatch.GetTimestamp() / Stopwatch.Frequency);
            }
            else
            {
                throw new NotSupportedException("Can't get uptime when IsHighResolution isen's supported!");
            }
        }
    }
}
