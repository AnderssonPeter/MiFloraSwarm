using Hangfire;
using Microsoft.Extensions.Logging;
using MiFloraGateway.Database;
using MiFloraGateway.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway.Sensors
{
    public interface IReadBatteryAndFirmwareCommand
    {
        void UpdateSchedule();
    }

    public class ReadBatteryAndFirmwareCommand : IReadBatteryAndFirmwareCommand
    {
        private readonly ILogger<DetectDeviceCommand> logger;
        private readonly IRecurringJobManager recurringJobManager;
        private readonly IDeviceLockManager deviceLockManager;
        private readonly DatabaseContext databaseContext;

        public ReadBatteryAndFirmwareCommand(ILogger<DetectDeviceCommand> logger, IRecurringJobManager recurringJobManager, IDeviceLockManager deviceLockManager, DatabaseContext databaseContext)
        {
            this.logger = logger;
            this.recurringJobManager = recurringJobManager;
            this.deviceLockManager = deviceLockManager;
            this.databaseContext = databaseContext;
        }

        public void UpdateSchedule()
        {
            
        }

        private async Task<IEnumerable<int>> CommandAsync(CancellationToken cancellationToken)
        {

        }

    }
}
