using Microsoft.Extensions.Logging;
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
        private readonly ILogger<DeviceLockManager> logger;
        AsyncLock innerLock = new AsyncLock();

        public DeviceLockManager(ILogger<DeviceLockManager> logger)
        {
            this.logger = logger;
        }

        public async Task<IDisposable> LockAsync(CancellationToken token = default)
        {
            logger.LogTrace("Acquiring lock");
            var locker = await innerLock.LockAsync(token);
            logger.LogTrace("Lock acquired");
            return locker;
        }
    }
}
