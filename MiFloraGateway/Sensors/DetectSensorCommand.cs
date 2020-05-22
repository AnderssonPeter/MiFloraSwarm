using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiFloraGateway.Database;
using MiFloraGateway.Devices;
using Polly;

namespace MiFloraGateway.Sensors
{
    public class DetectSensorCommand : IDetectSensorCommand
    {
        private readonly ILogger<DetectSensorCommand> logger;
        private readonly IDeviceLockManager deviceLockManager;
        private readonly DatabaseContext databaseContext;
        private readonly IDeviceCommunicationService deviceService;
        private readonly CancellationToken cancellationToken;

        public DetectSensorCommand(ILogger<DetectSensorCommand> logger, IDeviceCommunicationService deviceService,
                                   IDeviceLockManager deviceLockManager, DatabaseContext databaseContext,
                                   IJobCancellationToken cancellationToken)
        {
            this.logger = logger;
            this.deviceLockManager = deviceLockManager;
            this.databaseContext = databaseContext;
            this.deviceService = deviceService;
            this.cancellationToken = cancellationToken.ShutdownToken;
        }

        public async Task<int[]> ScanAsync()
        {
            int retryCount = 3;
            int delayAfterFailure = 5;
            logger.LogTrace("CommandAsync({retryCount}, {delayAfterFailure})", retryCount, delayAfterFailure);
            var token = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, new CancellationTokenSource(1000 * 30).Token).Token;

            var devices = await databaseContext.Devices.ToArrayAsync();
            var policy = Policy.Handle<HttpRequestException>().Or<OperationCanceledException>().WaitAndRetryAsync(retryCount, i => TimeSpan.FromSeconds(delayAfterFailure));
            using (await deviceLockManager.LockAsync(token))
            {
                var scanTasks = devices.Select(async device =>
                {
                    using (var logEntry = databaseContext.AddLogEntry(LogEntryEvent.Scan, device: device))
                    {
                        var result = await policy.ExecuteAndCaptureAsync(() => deviceService.ScanAsync(device, token));
                        if (result.Outcome == OutcomeType.Successful)
                        {
                            var sensorInfos = result.Result;
                            logger.LogInformation("Scan using {device} was successful, found {number} sensors", device, sensorInfos.Count());
                            foreach (var sensorInfo in sensorInfos)
                            {
                                var sensor = databaseContext.ChangeTracker.Entries<Sensor>().SingleOrDefault(x => x.Entity.MACAddress == sensorInfo.MACAddress)?.Entity;
                                if (sensor == null)
                                    sensor = await databaseContext.Sensors.SingleOrDefaultAsync(x => x.MACAddress == sensorInfo.MACAddress);

                                //If the device does not exist
                                if (sensor == null)
                                {
                                    sensor = new Sensor
                                    {
                                        MACAddress = sensorInfo.MACAddress,
                                        Name = sensorInfo.Name,
                                        Tags = new Collection<SensorTag> {
                                            new SensorTag { Tag = PredefinedTags.Added, Value = DateTime.Now.ToString("g") },
                                            new SensorTag { Tag = PredefinedTags.Source, Value = "Scan" }
                                        }
                                    };
                                    logger.LogInformation("Added sensor with {MACAddress}", sensorInfo.MACAddress);
                                    databaseContext.Sensors.Add(sensor);
                                }
                                databaseContext.DeviceSensorDistances.Add(new DeviceSensorDistance
                                {
                                    Device = device,
                                    Sensor = sensor,
                                    When = DateTime.Now,
                                    Rssi = sensorInfo.Rssi
                                });
                            }

                            var foundSensorIds = sensorInfos.Select(x => x.MACAddress);
                            var missedSensors = databaseContext.GetLatestSensorsForDevice(device).Where(sensor => !foundSensorIds.Contains(sensor.MACAddress));
                            foreach (var missedSensor in missedSensors)
                            {
                                //We found a sensor using this device before but not this time, so we should not use this device to scan for it again!
                                databaseContext.DeviceSensorDistances.Add(new DeviceSensorDistance
                                {
                                    Device = device,
                                    Sensor = missedSensor,
                                    When = DateTime.Now,
                                    Rssi = null
                                });

                                logger.LogInformation("Added missed sensor with {MACAddress}", missedSensor.MACAddress);
                            }
                            logEntry.Success();
                        }
                        else
                        {
                            logger.LogError(result.FinalException, "Failed to scan for sensors using {device}", device);
                            logEntry.Failure(result.FinalException.ToString());
                        }
                    }
                });
                await Task.WhenAll(scanTasks);
                var addedSensors = databaseContext.ChangeTracker.Entries<Sensor>().Where(x => x.State == EntityState.Added).Select(x => x.Entity).ToArray();
                logger.LogInformation("Saving changes");
                await databaseContext.SaveChangesAsync();
                return addedSensors.Select(x => x.Id).ToArray();
            }
        }
    }
}
