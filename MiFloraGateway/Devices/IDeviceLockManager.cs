using MiFloraGateway.Database;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway.Devices
{
    public interface IDeviceLockManager
    {
        Task<IDisposable> LockAsync(CancellationToken token = default);
    }
}
