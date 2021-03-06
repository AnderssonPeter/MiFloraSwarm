﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiFlora.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CoreFlora
{
    public class DiscoverBackgroundService : IHostedService
    {
        private readonly ILogger<DiscoverBackgroundService> logger;
        private readonly IHostingPort hostingPort;
        private readonly JsonSerializerOptions jsonSerializerOptions;
        CancellationTokenSource? cancellationTokenSource;
        Task? backgroundTask;

        public DiscoverBackgroundService(ILogger<DiscoverBackgroundService> logger, IHostingPort hostingPort, IOptions<JsonOptions> options)
        {
            this.logger = logger;
            this.hostingPort = hostingPort;
            this.jsonSerializerOptions = options.Value.JsonSerializerOptions;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationTokenSource = new CancellationTokenSource();
            backgroundTask = Run(cancellationTokenSource.Token);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (cancellationTokenSource == null)
                throw new InvalidOperationException("Can't stop unless started first!");
            cancellationTokenSource.Cancel();
            cancellationTokenSource = null;
            return Task.CompletedTask;
        }

        private async Task Run(CancellationToken cancellationToken)
        {
            const int serverPort = 16555;
            const int clientPort = 16556;
            logger.LogInformation("Starting UDPClient");
            using (var client = new UdpClient())
            using (cancellationToken.Register(() => client.Close()))
            {
                client.ExclusiveAddressUse = false;
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                client.Client.Bind(new IPEndPoint(IPAddress.Any, clientPort));
                logger.LogInformation("UDPClient started");
                while (!cancellationToken.IsCancellationRequested)
                {
                    var receiveResult = await client.ReceiveAsync().ConfigureAwait(false);
                    
                    logger.LogDebug($"Received {receiveResult.Buffer.Length} bytes from {receiveResult.RemoteEndPoint}");
                    try
                    {
                        var value = JsonSerializer.Deserialize<DeviceDiscoveryRequest>(receiveResult.Buffer, this.jsonSerializerOptions);
                        logger.LogInformation($"Received message from: {receiveResult.RemoteEndPoint}, name: {value.Name}, version: {value.Version}");
                        //Respond!
                        var assemblyName = Assembly.GetExecutingAssembly().GetName();                        
                        var response = JsonSerializer.SerializeToUtf8Bytes(new DeviceDiscoveryResponse { Name = assemblyName.Name!, Version = assemblyName.Version!, Port = this.hostingPort.Port }, this.jsonSerializerOptions);
                        await client.SendAsync(response, response.Length, new IPEndPoint(IPAddress.Broadcast, serverPort));
                        logger.LogInformation("Response sent");
                    }
                    catch(Exception ex)
                    {
                        logger.LogError(ex, "Failed to respond to discovery request!");
                    }
                    
                }
            }

        }
    }
}
