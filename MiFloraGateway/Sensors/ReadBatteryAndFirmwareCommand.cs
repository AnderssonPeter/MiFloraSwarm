using Hangfire;
using Hangfire.Console;
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
    public class ReadBatteryAndFirmwareCommand : IReadBatteryAndFirmwareCommand
    {
        private readonly ILogger<ReadBatteryAndFirmwareCommand> logger;
        private readonly IDeviceLockManager deviceLockManager;
        private readonly DatabaseContext databaseContext;
        private readonly IDeviceService deviceService;
        private readonly IJobManager jobManager;
        private readonly CancellationToken cancellationToken;

        public ReadBatteryAndFirmwareCommand(ILogger<ReadBatteryAndFirmwareCommand> logger,
            IDeviceLockManager deviceLockManager, DatabaseContext databaseContext, 
            IDeviceService deviceService, IJobManager jobManager,
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
                    var devices = await databaseContext.DeviceSensorDistances
                                                       .Where(x => x.Sensor == sensor)
                                                       .GroupBy(x => x.Device, (key, x) => x.OrderByDescending(d => d.When).First())
                                                       .OrderByDescending(x => x.Rssi)
                                                       .Select(x => x.Device).ToListAsync(cancellationToken);
                    foreach(var device in devices)
                    {
                        try
                        {
                            logger.LogInformation("Trying to get battery and version for {sensor} using {device}", sensor, device);
                            var result = await deviceService.GetBatteryAndVersionAsync(device.IPAddress, sensor.MACAddress, cancellationToken);
                            databaseContext.DeviceSensorDistances.Add(new DeviceSensorDistance { Device = device, Sensor = sensor, When = DateTime.Now, Rssi = result.Rssi });
                            databaseContext.SensorBatteryReadings.Add(new SensorBatteryAndVersionReading { Sensor = sensor, When = DateTime.Now, Battery = result.Battery, Version = result.Version });
                            await databaseContext.SaveChangesAsync(cancellationToken);
                            logger.LogInformation("Saved new battery and version values");

                            logger.LogInformation("Triggering a send of the new values!");
                            jobManager.Start<ISendValuesCommand>(command => command.CommandAsync(sensor.Id));
                            
                            break; //We managed to scan the sensor successfully no need to try any other devices!
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
