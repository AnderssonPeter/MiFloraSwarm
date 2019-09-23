﻿using Hangfire;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiFloraGateway.Database;
using MiFloraGateway.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiFloraGateway.Sensors
{
    public class ReadValuesCommand : IReadValuesCommand
    {
        private readonly ILogger<ReadBatteryAndFirmwareCommand> logger;
        private readonly IDeviceLockManager deviceLockManager;
        private readonly DatabaseContext databaseContext;
        private readonly IDeviceService deviceService;
        private readonly IBackgroundJobClient backgroundJobClient;

        public ReadValuesCommand(ILogger<ReadBatteryAndFirmwareCommand> logger,
            IDeviceLockManager deviceLockManager, DatabaseContext databaseContext,
            IDeviceService deviceService, IBackgroundJobClient backgroundJobClient)
        {
            this.logger = logger;
            this.deviceLockManager = deviceLockManager;
            this.databaseContext = databaseContext;
            this.deviceService = deviceService;
            this.backgroundJobClient = backgroundJobClient;
        }
        public async Task CommandAsync(PerformContext context)
        {
            var logger = new HangfireConsoleLogger(context, this.logger);
            var cancellationToken = context.CancellationToken.ShutdownToken;
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
                    foreach (var device in devices)
                    {
                        try
                        {
                            logger.LogInformation("Trying to get values for {sensor} using {device}", sensor, device);
                            var result = await deviceService.GetValuesAsync(device.IPAddress, sensor.MACAddress, cancellationToken);
                            databaseContext.DeviceSensorDistances.Add(new DeviceSensorDistance { Device = device, Sensor = sensor, When = DateTime.Now, Rssi = result.Rssi });
                            databaseContext.SensorDataReadings.Add(new SensorDataReading{ Sensor = sensor, When = DateTime.Now, Brightness = result.Brightness, Conductivity = result.Conductivity, Moisture = result.Moisture, Temperature = result.Temperature });
                            await databaseContext.SaveChangesAsync(cancellationToken);
                            logger.LogInformation("Saved new sensor values");

                            logger.LogInformation("Triggering a send of the new values!");
                            backgroundJobClient.ContinueJobWith<ISendValuesCommand>(context.BackgroundJob.Id, x => x.CommandAsync(null, sensor.Id));
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