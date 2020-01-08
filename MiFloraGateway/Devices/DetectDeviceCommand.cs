using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiFloraGateway.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway.Devices
{
    public class DetectDeviceCommand : IDetectDeviceCommand
    {
        private readonly ILogger<DetectDeviceCommand> logger;
        private readonly DatabaseContext databaseContext;
        private readonly IDeviceService deviceService;
        private readonly IDeviceLockManager deviceLockManager;

        public DetectDeviceCommand(ILogger<DetectDeviceCommand> logger, IDeviceLockManager deviceLockManager, DatabaseContext databaseContext, IDeviceService deviceService)
        {
            this.logger = logger;
            this.databaseContext = databaseContext;
            this.deviceService = deviceService;
            this.deviceLockManager = deviceLockManager;
        }

        public async Task<IEnumerable<int>> ScanAsync(CancellationToken cancellationToken)
        {
            logger.LogTrace("ScanAsync");
            var token = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, new CancellationTokenSource(3000).Token).Token;
            var ids = new List<Device>();
            const int port = 16555;
            logger.LogDebug("Starting UDPClient");
            using (var client = new UdpClient())
            using (var logEntry = databaseContext.AddLogEntry(LogEntryEvent.Scan))
            using (cancellationToken.Register(() => client.Close()))
            {
                try
                {
                    client.ExclusiveAddressUse = false;
                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    client.Client.Bind(new IPEndPoint(IPAddress.Any, port));
                    logger.LogDebug("UDPClient started");
                    var listeningTask = client.ReceiveAsync();
                    var sendTask = sendScanAsync(client, port, token);
                    var tasks = new List<Task> { listeningTask, sendTask };
                    while (!token.IsCancellationRequested)
                    {
                        var completedTask = await Task.WhenAny(tasks);
                        tasks.Remove(completedTask);
                        logger.LogDebug("Task completed");
                        if (completedTask == listeningTask)
                        {
                            if (completedTask.Exception != null)
                            {
                                logger.LogCritical(completedTask.Exception, "Listening task crashed!");
                                throw new Exception("Listen task caused a exception", completedTask.Exception);
                            }
                            var content = Encoding.UTF8.GetString(listeningTask.Result.Buffer);
                            logger.LogInformation($"Received \"{content}\" from {listeningTask.Result.RemoteEndPoint}");
                            if (content.StartsWith("MiFlora-Client"))
                            {
                                tasks.Add(handleClientAsync(listeningTask.Result.RemoteEndPoint.Address.ToString(), token));
                            }
                            listeningTask = client.ReceiveAsync();
                            tasks.Add(listeningTask);
                        }
                        else if (completedTask == sendTask)
                        {
                            if (completedTask.Exception != null)
                            {
                                logger.LogCritical(completedTask.Exception, "Send task crashed!");
                                throw new Exception("Send task caused a exception", completedTask.Exception);
                            }
                            logger.LogInformation("Send task finished!");
                        }
                        else
                        {
                            if (completedTask.Exception != null)
                            {
                                logger.LogCritical(completedTask.Exception, "Handle client task crashed!");
                            }
                            logger.LogInformation("Handle client task finished!");
                            ids.Add(((Task<Device>)completedTask).Result);
                        }
                    }
                    logEntry.Success("Scan successfully completed");
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "Failed to secan for new devices");
                    logEntry.Failure(ex.ToString());
                }
            }
            logger.LogInformation("Saving changes");
            await databaseContext.SaveChangesAsync(cancellationToken);
            return ids.Select(x => x.Id);
        }

        private async Task sendScanAsync(UdpClient client, int port, CancellationToken cancellationToken)
        {
            var message = "MiFlora-Server-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var buffer = Encoding.UTF8.GetBytes(message);
            for (var i = 0; i < 5; i++)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("Sending broadcast message to find devices.");
                    await client.SendAsync(buffer, buffer.Length, new IPEndPoint(IPAddress.Broadcast, port));
                    await Task.Delay(300, cancellationToken);
                }
            }
        }

        private async Task<Device> handleClientAsync(string clientAddress, CancellationToken cancellationToken)
        {
            logger.LogTrace("HandleClientAsync");
            try
            {
                logger.LogInformation($"Adding device {clientAddress}");
                using (await deviceLockManager.LockAsync(cancellationToken))
                {
                    var result = await deviceService.GetDeviceInfoAsync(clientAddress, cancellationToken);
                    var device = await databaseContext.Devices.SingleOrDefaultAsync(x => x.MACAddress == result.MACAddress);
                    if (device == null)
                    {
                        device = new Device()
                        {
                            IPAddress = clientAddress,
                            MACAddress = result.MACAddress,
                            Name = "",
                            Tags = new[] {
                                new DeviceTag { Tag = PredefinedTags.Added, Value = DateTime.Now.ToString("g") },
                                new DeviceTag { Tag = PredefinedTags.Source, Value = "ScanOrAutoConnect" }
                            }
                        };
                        databaseContext.Add(device);
                        logger.LogInformation("Added new device with {MACAddress}", result.MACAddress);
                    }
                    else if (device.IPAddress != clientAddress)
                    {
                        device.IPAddress = clientAddress;
                        databaseContext.DevicesTags.Add(new DeviceTag { Device = device, Tag = PredefinedTags.IPAddressUpdated, Value = DateTime.Now.ToString("g") });

                        logger.LogInformation("Updated device with {MACAddress}", result.MACAddress);
                    }
                    return device;
                }
            }
            catch (HttpRequestException ex)
            {
                logger.LogCritical(ex, "Failed to communicate with MiFlora device!");
                throw;
            }
        }
    }
}
