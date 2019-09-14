using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway.Devices
{
    public interface IDetectDeviceCommand
    {
        Task<IEnumerable<int>> ScanAsync(CancellationToken cancellationToken);
    }
}
