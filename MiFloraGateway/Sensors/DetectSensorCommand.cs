using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiFloraGateway.Database;
using MiFloraGateway.Devices;
using Polly;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway.Sensors
{
    public class DetectSensorCommand : IDetectSensorCommand
    {
        private readonly ILogger<DetectSensorCommand> logger;
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly IDeviceLockManager deviceLockManager;
        private readonly DatabaseContext databaseContext;
        private readonly IDeviceService deviceService;
        private int retryCount;
        private int delayAfterFailure;
        public DetectSensorCommand(ILogger<DetectSensorCommand> logger, IDeviceService deviceService, IBackgroundJobClient backgroundJobClient, IDeviceLockManager deviceLockManager, DatabaseContext databaseContext)
        {
            this.logger = logger;
            this.backgroundJobClient = backgroundJobClient;
            this.deviceLockManager = deviceLockManager;
            this.databaseContext = databaseContext;
            this.deviceService = deviceService;
        }

        public Task<IEnumerable<int>> ScanAsync(int retryCount = 3, int delayAfterFailure = 5, CancellationToken cancellationToken = default)
        {
            this.retryCount = retryCount;
            this.delayAfterFailure = delayAfterFailure;
            return backgroundJobClient.RunTaskAsync(CommandAsync, cancellationToken);
        }

        private async Task<IEnumerable<int>> CommandAsync(CancellationToken cancellationToken)
        {
            var token = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, new CancellationTokenSource(3000).Token).Token;

            var devices = await databaseContext.Devices.ToArrayAsync();
            var policy = Policy.Handle<HttpRequestException>().Or<OperationCanceledException>().WaitAndRetryAsync(retryCount, i => TimeSpan.FromSeconds(delayAfterFailure));
            using (await deviceLockManager.LockAsync(token))
            {
                var scanTasks = devices.Select(async device => {
                    using (var logEntry = databaseContext.AddLogEntry(LogEntryEvent.Scan, device: device))
                    {
                        var result = await policy.ExecuteAndCaptureAsync(() => deviceService.ScanAsync(device.IPAddress, token));
                        if (result.Outcome == OutcomeType.Successful)
                        {
                            foreach (var sensorInfo in result.Result)
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
                                    databaseContext.Sensors.Add(sensor);
                                }
                                await databaseContext.DeviceSensorDistances.AddAsync(new DeviceSensorDistance
                                {
                                    Device = device,
                                    Sensor = sensor,
                                    When = DateTime.Now,
                                    Rssi = sensorInfo.Rssi
                                });
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
                var addedSensors = databaseContext.ChangeTracker.Entries<Sensor>().Where(x => x.State == EntityState.Added).Select(x => x.Entity);
                await databaseContext.SaveChangesAsync();
                return addedSensors.Select(x => x.Id);
            }
        }
    }
}
