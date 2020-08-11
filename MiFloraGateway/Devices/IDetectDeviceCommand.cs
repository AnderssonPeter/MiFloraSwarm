using System.Threading.Tasks;
using Hangfire;

namespace MiFloraGateway.Devices
{
    public interface IDetectDeviceCommand
    {
        [AutomaticRetry(Attempts = 0), JobDisplayName("Scan for new devices")]
        Task<int[]> ScanAsync();
    }
}
