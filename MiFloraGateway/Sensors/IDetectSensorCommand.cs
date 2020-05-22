using System.Threading.Tasks;
using Hangfire;

namespace MiFloraGateway.Sensors
{
    public interface IDetectSensorCommand
    {
        [AutomaticRetry(Attempts = 0), JobDisplayName("Scan for new sensors")]
        Task<int[]> ScanAsync();
    }
}
