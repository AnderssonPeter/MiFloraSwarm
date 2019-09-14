//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Net.Sockets;
//using System.Reflection;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using MiFloraGateway.Database;

//namespace MiFloraGateway.Devices
//{
//    public class DeviceDetector : IDeviceDetector, IHostedService
//    {
//        Task backgroundTask;
//        CancellationTokenSource cancellationTokenSource;
//        private readonly ILogger<DeviceDetector> logger;
//        private readonly IServiceScopeFactory serviceScopeFactory;
//        private readonly IDeviceLockManager deviceLockManager;
//        private readonly Nito.AsyncEx.AsyncAutoResetEvent scanTrigger = new Nito.AsyncEx.AsyncAutoResetEvent();

//        public DeviceDetector(ILogger<DeviceDetector> logger, IServiceScopeFactory serviceScopeFactory, IDeviceLockManager deviceLockManager)
//        {
//            this.logger = logger;
//            this.serviceScopeFactory = serviceScopeFactory;
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

//        public void Scan()
//        {
//            logger.LogTrace("Scan");
//            scanTrigger.Set();
//        }

//        private async Task RunAsync(CancellationToken cancellationToken)
//        {
//            logger.LogTrace("RunAsync");
//            using (var scope = serviceScopeFactory.CreateScope())
//            {
//                var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
//                var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();
//                const int port = 16555;
//                logger.LogDebug("Starting UDPClient");
//                using (UdpClient client = new UdpClient())
//                {
//                    client.ExclusiveAddressUse = false;
//                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
//                    client.Client.Bind(new IPEndPoint(IPAddress.Any, port));
//                    logger.LogDebug("UDPClient started");

//                    var listeningTask = client.ReceiveAsync();
//                    var scanTask = sendScanAsync(client, port, cancellationToken);
//                    List<Task> tasks = new List<Task> { listeningTask, scanTask };
//                    while (!cancellationToken.IsCancellationRequested)
//                    {
//                        var completedTask = await Task.WhenAny(tasks);
//                        logger.LogDebug("Task completed");
//                        tasks.Remove(completedTask);
//                        if (completedTask == listeningTask)
//                        {
//                            if (completedTask.Exception != null)
//                            {
//                                logger.LogCritical(completedTask.Exception, "Listening task crashed!");
//                                throw new Exception("Listen task caused a exception", completedTask.Exception);
//                            }
//                            var content = Encoding.UTF8.GetString(listeningTask.Result.Buffer);
//                            logger.LogInformation($"Recivied \"{content}\" from {listeningTask.Result.RemoteEndPoint}");
//                            if (content.StartsWith("MiFlora-Client"))
//                            {
//                                tasks.Add(handleClientAsync(deviceService, databaseContext, listeningTask.Result.RemoteEndPoint.Address.ToString(), cancellationToken));
//                            }
//                            listeningTask = client.ReceiveAsync();
//                            tasks.Add(listeningTask);
//                        }
//                        else if (completedTask == scanTask)
//                        {
//                            if (completedTask.Exception != null)
//                            {
//                                logger.LogCritical(completedTask.Exception, "Scan task crashed!");
//                                throw new Exception("Scan task caused a exception", completedTask.Exception);
//                            }
//                            logger.LogWarning("Scan task exited without a exception!");
//                        }
//                        else if (completedTask.Exception != null)
//                            logger.LogCritical(completedTask.Exception, "HandleClient task crashed!");
//                    }
//                }
//            }
//        }

//        private async Task sendScanAsync(UdpClient client, int port, CancellationToken cancellationToken)
//        {
//            while (!cancellationToken.IsCancellationRequested)
//            {
//                await scanTrigger.WaitAsync(cancellationToken);
//                var message = "MiFlora-Server-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
//                var buffer = Encoding.UTF8.GetBytes(message);
                
//                await client.SendAsync(buffer, buffer.Length, new IPEndPoint(IPAddress.Broadcast, port));
//                await Task.Delay(300);
//                /*await client.SendAsync(buffer, buffer.Length, new IPEndPoint(IPAddress.Broadcast, port));
//                await Task.Delay(300);
//                await client.SendAsync(buffer, buffer.Length, new IPEndPoint(IPAddress.Broadcast, port));
//                await Task.Delay(300);
//                await client.SendAsync(buffer, buffer.Length, new IPEndPoint(IPAddress.Broadcast, port));*/
//            }
//        }

//        private async Task handleClientAsync(IDeviceService deviceService, DatabaseContext databaseContext, string clientAddress, CancellationToken cancellationToken)
//        {
//            logger.LogTrace("HandleClientAsync");
//            try
//            {
//                using (await deviceLockManager.LockAsync(cancellationToken))
//                {
//                    var result = await deviceService.GetDeviceInfoAsync(clientAddress, cancellationToken);
//                    var device = await databaseContext.Devices.SingleOrDefaultAsync(x => x.MACAddress == result.MACAddress);
//                    if (device == null)
//                    {
//                        device = new Device()
//                        {
//                            IPAddress = clientAddress,
//                            MACAddress = result.MACAddress,
//                            Name = "",
//                            Tags = new[] {
//                            new DeviceTag { Tag = PredefinedTags.Added, Value = DateTime.Now.ToString("g") },
//                            new DeviceTag { Tag = PredefinedTags.Source, Value = "ScanOrAutoConnect" }
//                        }
//                        };
//                        databaseContext.Add(device);
//                        logger.LogInformation("Added device");
//                    }
//                    else if (device.IPAddress != clientAddress)
//                    {
//                        device.IPAddress = clientAddress;
//                        databaseContext.DevicesTags.Add(new DeviceTag { Device = device, Tag = PredefinedTags.IPAddressUpdated, Value = DateTime.Now.ToString("g") });
//                        logger.LogInformation("Device updated");
//                    }
//                    await databaseContext.SaveChangesAsync(cancellationToken);
//                }
//            }
//            catch (HttpRequestException ex)
//            {
//                logger.LogCritical(ex, "Failed to communicate with MiFlora device!");
//            }
//        }
//    }
//}
