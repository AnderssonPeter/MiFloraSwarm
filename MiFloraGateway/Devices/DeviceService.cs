using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MiFloraGateway.Devices
{
    public class DeviceService : IDeviceService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<DeviceService> logger;

        public DeviceService(HttpClient httpClient, ILogger<DeviceService> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        private async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("GetAsync({url})", url);
            var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, new CancellationTokenSource(12 * 1000).Token);
            var result = await httpClient.GetAsync(url, tokenSource.Token);
            logger.LogDebug("Http request to {url} completed with {StatusCode}", url, result.StatusCode);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsAsync<T>(tokenSource.Token);
        }

        public Task<IEnumerable<SensorInfo>> ScanAsync(string ipAddress, CancellationToken cancellationToken = default) => 
            GetAsync<IEnumerable<SensorInfo>>($"http://{ipAddress}/sensors/scan", cancellationToken);

        public Task<BatteryAndVersionInfo> GetBatteryAndVersionAsync(string ipAddress, string sensorAddress, CancellationToken cancellationToken = default) =>
            GetAsync<BatteryAndVersionInfo>($"http://{ipAddress}/sensors/{sensorAddress}/info", cancellationToken);

        public Task<ValuesInfo> GetValuesAsync(string ipAddress, string sensorAddress, CancellationToken cancellationToken = default) =>
            GetAsync<ValuesInfo>($"http://{ipAddress}/sensors/{sensorAddress}/info", cancellationToken);

        public Task<DeviceInfo> GetDeviceInfoAsync(string ipAddress, CancellationToken cancellationToken = default) =>
            GetAsync<DeviceInfo>($"http://{ipAddress}/device", cancellationToken);
    }

    public class DeviceInfo
    {
        public Version Version { get; set; }
        public string Name { get; set; }
        public TimeSpan Uptime { get; set; }
        public string MACAddress { get; set; }
    }

    public class ValuesInfo
    {
        public float Temperature { get; set; }
        public int Brightness { get; set; }
        public int Moisture { get; set; }
        public int Conductivity { get; set; }
        public int Rssi { get; set; }
    }

    public class BatteryAndVersionInfo
    {
        public byte Battery { get; set; }
        public Version Version { get; set; }
        public int Rssi { get; set; }
    }

    public class SensorInfo
    {
        public string Name { get; set; }
        public string MACAddress { get; set; }
        public int Rssi { get; set; }
    }

}
