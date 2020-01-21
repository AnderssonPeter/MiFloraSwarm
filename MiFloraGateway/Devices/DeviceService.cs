using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiFlora.Common;

namespace MiFloraGateway.Devices
{
    public class DeviceService : IDeviceService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<DeviceService> logger;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public DeviceService(HttpClient httpClient, ILogger<DeviceService> logger, IOptions<JsonOptions> options)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this.jsonSerializerOptions = options.Value.JsonSerializerOptions;
        }

        private async Task<T> GetAsync<T>(IPEndPoint endpoint, string urlPart, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("GetAsync({endpoint}, {urlPart})", endpoint, urlPart);
            var url = $"http://{endpoint.Address}:{endpoint.Port}/{urlPart}";
            var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, new CancellationTokenSource(12 * 1000).Token);
            var result = await httpClient.GetAsync(url, tokenSource.Token);
            logger.LogDebug("Http request to {url} completed with {StatusCode}", url, result.StatusCode);
            result.EnsureSuccessStatusCode();
            using (var stream = await result.Content.ReadAsStreamAsync())
            {
                return await JsonSerializer.DeserializeAsync<T>(stream, this.jsonSerializerOptions, cancellationToken);
            }
        }

        public Task<IEnumerable<SensorInfo>> ScanAsync(IPEndPoint endpoint, CancellationToken cancellationToken = default) => 
            GetAsync<IEnumerable<SensorInfo>>(endpoint, "sensors/scan", cancellationToken);

        public Task<BatteryAndVersionInfo> GetBatteryAndVersionAsync(IPEndPoint endpoint, string sensorAddress, CancellationToken cancellationToken = default) =>
            GetAsync<BatteryAndVersionInfo>(endpoint, $"sensors/{sensorAddress}/info", cancellationToken);

        public Task<ValuesInfo> GetValuesAsync(IPEndPoint endpoint, string sensorAddress, CancellationToken cancellationToken = default) =>
            GetAsync<ValuesInfo>(endpoint, $"sensors/{sensorAddress}/info", cancellationToken);

        public Task<DeviceInfo> GetDeviceInfoAsync(IPEndPoint endpoint, CancellationToken cancellationToken = default) =>
            GetAsync<DeviceInfo>(endpoint, "device", cancellationToken);
    }
}
