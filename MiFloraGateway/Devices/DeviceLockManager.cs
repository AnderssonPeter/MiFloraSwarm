using MiFloraGateway.Database;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway.Devices
{
    public class DeviceLockManager : IDeviceLockManager
    {
        AsyncLock innerLock = new AsyncLock();

        public Task<IDisposable> LockAsync(CancellationToken token = default)
        {
            return innerLock.LockAsync(token);
        }
    }
}
