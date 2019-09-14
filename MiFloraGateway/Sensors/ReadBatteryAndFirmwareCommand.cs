using Hangfire;
using Microsoft.EntityFrameworkCore;
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
        void Initialize();
    }

    public class ReadBatteryAndFirmwareCommand : IReadBatteryAndFirmwareCommand
    {
        private readonly ILogger<ReadBatteryAndFirmwareCommand> logger;
        private readonly IRecurringJobManager recurringJobManager;
        private readonly IDeviceLockManager deviceLockManager;
        private readonly DatabaseContext databaseContext;
        private readonly ISettingsManager settingsManager;
        private readonly IDeviceService deviceService;

        public ReadBatteryAndFirmwareCommand(ILogger<ReadBatteryAndFirmwareCommand> logger, IRecurringJobManager recurringJobManager, 
            IDeviceLockManager deviceLockManager, DatabaseContext databaseContext, 
            ISettingsManager settingsManager, IDeviceService deviceService)
        {
            this.logger = logger;
            this.recurringJobManager = recurringJobManager;
            this.deviceLockManager = deviceLockManager;
            this.databaseContext = databaseContext;
            this.settingsManager = settingsManager;
            this.deviceService = deviceService;
        }

        public void Initialize()
        {
            UpdateSchedule(settingsManager.Get(Settings.UpdateBatteryCron));
            settingsManager.WatchForChanges((_, cronExpression) => UpdateSchedule(cronExpression));
        }

        private void UpdateSchedule(string cronExpression)
        {
            recurringJobManager.AddOrUpdate("ReadBatteryAndFirmwareVersion", () => CommandAsync(null), cronExpression);
        }

        private async Task CommandAsync(JobCancellationToken jobCancellationToken)
        {
            var cancellationToken = jobCancellationToken.ShutdownToken;
            using (await deviceLockManager.LockAsync(cancellationToken))
            {
                var scanQueue2 = await databaseContext.Sensors.Include(x => x.DeviceDistances)
                                                             .ThenInclude(x => x.Device)
                                                             .ToDictionaryAsync(x => x, x => x.DeviceDistances.OrderBy(dd => dd.Rssi) //This is wrong we need to only get the latest distance for each device!
                                                                                                              .Select(dd => dd.Device), cancellationToken);

                var scanQueue = await databaseContext.Sensors.Include(x => x.DeviceDistances)
                                                             .ThenInclude(x => x.Device)
                                                             .ToDictionaryAsync(x => x, x => x.DeviceDistances.OrderBy(dd => dd.Rssi) //This is wrong we need to only get the latest distance for each device!
                                                                                                              .Select(dd => dd.Device), cancellationToken);
                foreach (var (sensor, devices) in scanQueue)
                {
                    foreach (var device in devices)
                    {
                        try
                        {
                            logger.LogInformation("Reading the sensor values from {sensor} using {device}", sensor, device);
                            var result = await deviceService.GetBatteryAndVersionAsync(device.IPAddress, sensor.MACAddress, cancellationToken);
                            databaseContext.DeviceSensorDistances.Add(new DeviceSensorDistance { Device = device, Sensor = sensor, When = DateTime.Now, Rssi = result.Rssi });
                            databaseContext.SensorBatteryReadings.Add(new SensorBatteryAndVersionReading { Sensor = sensor, When = DateTime.Now, Battery = result.Battery, Version = result.Version });
                            await databaseContext.SaveChangesAsync(cancellationToken);
                            logger.LogInformation("Saved new sensor values");
                            break; //We managed to scan the sensor successfully no need to 
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to read the sensor values from {sensor} using {device}", sensor, device);
                        }
                    }
                }

            }
        }

    }
}
