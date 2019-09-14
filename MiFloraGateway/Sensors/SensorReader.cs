//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using MiFloraGateway.Database;
//using MiFloraGateway.Devices;

//namespace MiFloraGateway.Sensors
//{
//    public interface ISensorReader
//    {

//    }

//    public class SensorReader : IHostedService
//    {
//        Task backgroundTask;
//        CancellationTokenSource cancellationTokenSource;
//        private readonly IDeviceService deviceService;
//        private readonly ILogger<SensorReader> logger;
//        private readonly DatabaseContext databaseContext;
//        private readonly IDeviceLockManager deviceLockManager;

//        public SensorReader(IDeviceService deviceService, ILogger<SensorReader> logger, DatabaseContext databaseContext, IDeviceLockManager deviceLockManager)
//        {
//            this.deviceService = deviceService;
//            this.logger = logger;
//            this.databaseContext = databaseContext;
//            this.deviceLockManager = deviceLockManager;
//        }

//        public Task StartAsync(CancellationToken cancellationToken)
//        {
//            logger.LogInformation("StartAsync");
//            cancellationTokenSource = new CancellationTokenSource();
//            backgroundTask = RunAsync(cancellationTokenSource.Token);
//            return Task.CompletedTask;
//        }

//        public async Task StopAsync(CancellationToken cancellationToken)
//        {
//            logger.LogInformation("StopAsync");
//            cancellationTokenSource.Cancel();
//            cancellationTokenSource = null;
//            try
//            {
//                await backgroundTask;
//            }
//            catch (Exception ex)
//            {
//                //ignore any errors
//            }
//        }

//        private Task RunAsync(CancellationToken cancellationToken)
//        {
//            logger.LogTrace("RunAsync");
//            var tasks = new[] { getVaulesAsync(cancellationToken), updateSensorDistance(cancellationToken), getBatteryAndFirmware(cancellationToken) };
//            return Task.WhenAll(tasks);
//        }

//        private async Task getBatteryAndFirmware(CancellationToken cancellationToken)
//        {
//            logger.LogTrace("getBatteryAndFirmware");

//            while (!cancellationToken.IsCancellationRequested)
//            {
//                await Task.Delay(getBatteryAndFirmwareIntervall, cancellationToken);
//                using (deviceLockManager.LockAsync(cancellationToken))
//                {

//                }
//            }
//        }

//        private async Task updateSensorDistance(CancellationToken cancellationToken)
//        {
//            logger.LogTrace("updateSensorDistance");

//            while (!cancellationToken.IsCancellationRequested)
//            {
//                await Task.Delay(updateDistanceIntervall, cancellationToken);
//                using (deviceLockManager.LockAsync(cancellationToken))
//                {
                    
//                }
//            }
//        }

//        readonly TimeSpan scanIntervall = TimeSpan.FromMinutes(30);
//        readonly TimeSpan updateDistanceIntervall = TimeSpan.FromDays(1);
//        readonly TimeSpan getBatteryAndFirmwareIntervall = TimeSpan.FromDays(1);

//        private async Task getVaulesAsync(CancellationToken cancellationToken)
//        {
//            logger.LogTrace("getVaulesAsync");
//            while (!cancellationToken.IsCancellationRequested)
//            {
//                await Task.Delay(scanIntervall, cancellationToken);
//                using (deviceLockManager.LockAsync(cancellationToken))
//                {

//                    var scanQueue2 = await databaseContext.Sensors.Include(x => x.DeviceDistances)
//                                                                 .ThenInclude(x => x.Device)
//                                                                 .ToDictionaryAsync(x => x, x => x.DeviceDistances.OrderBy(dd => dd.Rssi) //This is wrong we need to only get the latest distance for each device!
//                                                                                                                  .Select(dd => dd.Device), cancellationToken);

//                    var scanQueue = await databaseContext.Sensors.Include(x => x.DeviceDistances)
//                                                                 .ThenInclude(x => x.Device)
//                                                                 .ToDictionaryAsync(x => x, x => x.DeviceDistances.OrderBy(dd => dd.Rssi) //This is wrong we need to only get the latest distance for each device!
//                                                                                                                  .Select(dd => dd.Device), cancellationToken);
//                    foreach (var (sensor, devices) in scanQueue)
//                    {
//                        foreach (var device in devices)
//                        {
//                            try
//                            {
//                                logger.LogInformation("Reading the sensor values from {sensor} using {device}", sensor, device);
//                                var result = await deviceService.GetValuesAsync(device.IPAddress, sensor.MACAddress, cancellationToken);
//                                databaseContext.DeviceSensorDistances.Add(new DeviceSensorDistance { Device = device, Sensor = sensor, When = DateTime.Now, Rssi = result.Rssi });
//                                databaseContext.SensorDataReadings.Add(new SensorDataReading { Sensor = sensor, When = DateTime.Now, Brightness = result.Brightness, Conductivity = result.Conductivity, Moisture = result.Moisture, Temperature = result.Temperature });
//                                await databaseContext.SaveChangesAsync(cancellationToken);
//                                logger.LogInformation("Saved new sensor values");
//                                break; //We managed to scan the sensor successfully no need to 
//                            }
//                            catch (Exception ex)
//                            {
//                                logger.LogError(ex, "Failed to read the sensor values from {sensor} using {device}", sensor, device);
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    }
//}
