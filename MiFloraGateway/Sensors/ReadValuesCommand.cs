using Hangfire;
using Hangfire.Server;
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
    public class ReadValuesCommand : IReadValuesCommand
    {
        private readonly ILogger<ReadBatteryAndFirmwareCommand> logger;
        private readonly IDeviceLockManager deviceLockManager;
        private readonly DatabaseContext databaseContext;
        private readonly IDeviceCommunicationService deviceService;
        private readonly IJobManager jobManager;
        private readonly CancellationToken cancellationToken;

        public ReadValuesCommand(ILogger<ReadBatteryAndFirmwareCommand> logger,
            IDeviceLockManager deviceLockManager, DatabaseContext databaseContext,
            IDeviceCommunicationService deviceService, IJobManager jobManager,
            ICancellationTokenAccessor cancellationTokenAccessor)
        {
            this.logger = logger;
            this.deviceLockManager = deviceLockManager;
            this.databaseContext = databaseContext;
            this.deviceService = deviceService;
            this.jobManager = jobManager;
            this.cancellationToken = cancellationTokenAccessor.Get();
        }

        public async Task CommandAsync()
        {
            logger.LogTrace("CommandAsync");
            using (await deviceLockManager.LockAsync(cancellationToken))
            {
                foreach (var sensor in await databaseContext.Sensors.ToListAsync())
                {
                    logger.LogInformation("Getting device priority for {sensor}", sensor);
                    var devices = await databaseContext.GetDeviceUsagePriority(sensor)
                                                       .ToListAsync(cancellationToken);
                    foreach (var device in devices)
                    {
                        try
                        {
                            logger.LogInformation("Trying to get values for {sensor} using {device}", sensor, device);
                            var result = await deviceService.GetValuesAsync(device, sensor.MACAddress, cancellationToken);
                            databaseContext.DeviceSensorDistances.Add(new DeviceSensorDistance { Device = device, Sensor = sensor, When = DateTime.Now, Rssi = result.Rssi });
                            databaseContext.SensorDataReadings.Add(new SensorDataReading{ Sensor = sensor, When = DateTime.Now, Brightness = result.Brightness, Conductivity = result.Conductivity, Moisture = result.Moisture, Temperature = result.Temperature });
                            logger.LogTrace("Saving changes");
                            await databaseContext.SaveChangesAsync(cancellationToken);
                            logger.LogInformation("Saved new sensor values");

                            logger.LogInformation("Triggering a send of the new values!");
                            jobManager.Start<ISendValuesCommand>(command => command.CommandAsync(sensor.Id));
                            break; //We managed to scan the sensor successfully no need to try any other devices!
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to read the sensor values from {sensor} using {device}", sensor, device);
                            //todo: if we failed x amount of times we should add a huge rssi so that we get lower priority!
                        }
                    }
                }
            }
        }
    }
}
