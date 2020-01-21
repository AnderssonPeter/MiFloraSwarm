using Hangfire;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway.Sensors
{
    public interface IDetectSensorCommand
    {
        [AutomaticRetry(Attempts = 0), JobDisplayName("Scan for new sensors")]
        Task<int[]> CommandAsync();
    }
}
