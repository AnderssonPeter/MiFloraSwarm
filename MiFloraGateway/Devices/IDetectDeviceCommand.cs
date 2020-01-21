using Hangfire;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway.Devices
{
    public interface IDetectDeviceCommand
    {
        [AutomaticRetry(Attempts = 0), JobDisplayName("Scan for new devices")]
        Task<int[]> ScanAsync();
    }
}
