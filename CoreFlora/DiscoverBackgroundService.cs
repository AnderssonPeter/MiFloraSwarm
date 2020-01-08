using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreFlora
{
    public class DiscoverBackgroundService : IHostedService
    {
        private readonly ILogger<DiscoverBackgroundService> logger;
        CancellationTokenSource cancellationTokenSource;
        Task backgroundTask;

        public DiscoverBackgroundService(ILogger<DiscoverBackgroundService> logger)
        {
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationTokenSource = new CancellationTokenSource();
            backgroundTask = Run(cancellationTokenSource.Token);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }

        private async Task Run(CancellationToken cancellationToken)
        {
            const int port = 16555;
            logger.LogInformation("Starting UDPClient");
            using (var client = new UdpClient())
            using (cancellationToken.Register(() => client.Close()))
            {
                client.ExclusiveAddressUse = false;
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                client.Client.Bind(new IPEndPoint(IPAddress.Any, port));
                logger.LogInformation("UDPClient started");
                while (!cancellationToken.IsCancellationRequested)
                {
                    var receiveResult = await client.ReceiveAsync().ConfigureAwait(false);
                    var content = Encoding.UTF8.GetString(receiveResult.Buffer);

                    logger.LogInformation($"Received \"{content}\" from {receiveResult.RemoteEndPoint}");
                    if (content.StartsWith("MiFlora-Server-"))
                    {
                        var response = Encoding.UTF8.GetBytes("MiFlora-Client-Core-" + Assembly.GetExecutingAssembly().GetName().Version.ToString());
                        await client.SendAsync(response, response.Length, new IPEndPoint(IPAddress.Broadcast, port));
                        logger.LogInformation("Response sent");
                    }
                }
            }

        }
    }
}
