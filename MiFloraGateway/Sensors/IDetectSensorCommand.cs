using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway.Sensors
{
    public interface IDetectSensorCommand
    {
        Task<IEnumerable<int>> ScanAsync(int retryCount = 3, int delayAfterFailure = 5, CancellationToken cancellationToken = default);
    }
}
