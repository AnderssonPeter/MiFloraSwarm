using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiFlora.Common;
using MiFloraGateway.Database;
using MiFloraGateway.Logs;

namespace MiFloraGateway.Devices
{
    public class DetectDeviceCommand : IDetectDeviceCommand
    {
        private readonly ILogger<DetectDeviceCommand> logger;
        private readonly DatabaseContext databaseContext;
        private readonly IDeviceCommunicationService deviceService;
        private readonly LogEntryHandler logEntryHandler;
        private readonly IDeviceLockManager deviceLockManager;
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly CancellationToken cancellationToken;

        public DetectDeviceCommand(ILogger<DetectDeviceCommand> logger, IDeviceLockManager deviceLockManager,
                                   DatabaseContext databaseContext, IDeviceCommunicationService deviceService,
                                   IOptions<JsonOptions> options, LogEntryHandler logEntryHandler,
                                   IJobCancellationToken cancellationToken)
        {
            this.logger = logger;
            this.databaseContext = databaseContext;
            this.deviceService = deviceService;
            this.logEntryHandler = logEntryHandler;
            this.deviceLockManager = deviceLockManager;
            this.jsonSerializerOptions = options.Value.JsonSerializerOptions;
            this.cancellationToken = cancellationToken.ShutdownToken;
        }

        public async Task<int[]> ScanAsync()
        {
            logger.LogTrace("ScanAsync");
            var token = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, new CancellationTokenSource(3000).Token).Token;
            var devices = new List<Device>();
            const int serverPort = 16555;
            const int clientPort = 16556;
            logger.LogDebug("Starting UDPClient");
            using (var client = new UdpClient())
            await using (var logEntry = logEntryHandler.AddLogEntry(LogEntryEvent.Scan))
            using (token.Register(() =>
            {
                logger.LogWarning("Timeout occurred closing udpClient");
                client.Close();
            }))
            {
                try
                {
                    client.ExclusiveAddressUse = false;
                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    client.Client.Bind(new IPEndPoint(IPAddress.Any, serverPort));
                    logger.LogDebug("UDPClient started");
                    var listeningTask = client.ReceiveAsync();
                    var sendTask = SendScanAsync(client, clientPort, token);
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
                            logger.LogInformation($"Received response from {listeningTask.Result.RemoteEndPoint}");
                            tasks.Add(HandleClientAsync(listeningTask.Result.RemoteEndPoint, listeningTask.Result.Buffer, token));
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
                                throw new Exception("Handle client task caused a exception", completedTask.Exception);
                            }
                            logger.LogInformation("Handle client task finished!");
                            var device = ((Task<Device?>)completedTask).Result;
                            if (device != null)
                            {
                                devices.Add(device);
                            }
                        }
                    }
                    logEntry.Success("Scan successfully completed");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to scan for new devices");
                    logEntry.Failure(ex.ToString());
                }
            }
            logger.LogInformation("Saving changes");
            await databaseContext.SaveChangesAsync(cancellationToken);
            return devices.Select(x => x.Id).ToArray();
        }

        private async Task SendScanAsync(UdpClient client, int port, CancellationToken cancellationToken)
        {
            var assembyName = Assembly.GetExecutingAssembly().GetName();
            var request = new DeviceDiscoveryRequest()
            {
                Name = assembyName.Name!,
                Version = assembyName.Version!
            };
            var buffer = JsonSerializer.SerializeToUtf8Bytes(request, this.jsonSerializerOptions);
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

        private async Task<Device?> HandleClientAsync(IPEndPoint endPoint, byte[] buffer, CancellationToken cancellationToken)
        {
            logger.LogTrace("HandleClientAsync");
            try
            {
                logger.LogInformation("Deserializing response");
                var response = JsonSerializer.Deserialize<DeviceDiscoveryResponse>(buffer, this.jsonSerializerOptions);
                var restEndpoint = new IPEndPoint(endPoint.Address, response.Port);
                logger.LogInformation($"Adding device {endPoint}");
                using (await deviceLockManager.LockAsync(cancellationToken))
                {
                    var result = await deviceService.GetDeviceInfoAsync(restEndpoint, cancellationToken);
                    var macAddress = result.MACAddress.ToFormattedString();
                    var device = await databaseContext.Devices.SingleOrDefaultAsync(x => x.MACAddress == macAddress);
                    if (databaseContext.ChangeTracker.Entries<Device>().Any(device => device.Entity.MACAddress == macAddress))
                    {
                        logger.LogInformation($"Device has already been queued!");
                        return null;
                    }
                    if (device == null)
                    {
                        device = new Device()
                        {
                            IPAddress = endPoint.Address.ToString(),
                            Port = response.Port,
                            MACAddress = macAddress,
                            Name = result.Name + " - " + macAddress,
                            Tags = new[] {
                                new DeviceTag { Tag = PredefinedTags.Added, Value = DateTime.Now.ToString("g") },
                                new DeviceTag { Tag = PredefinedTags.Source, Value = "ScanOrAutoConnect" }
                            }
                        };
                        databaseContext.Add(device);
                        logger.LogInformation("Added new device with {MACAddress}", result.MACAddress);
                    }
                    else if (device.IPAddress != endPoint.Address.ToString())
                    {
                        device.IPAddress = endPoint.Address.ToString();
                        device.Port = endPoint.Port;
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
