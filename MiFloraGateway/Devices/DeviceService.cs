using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MiFloraGateway.Devices
{
    public class DeviceService : IDeviceService
    {
        private readonly HttpClient httpClient;

        public DeviceService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        private async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default)
        {
            var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, new CancellationTokenSource(12 * 1000).Token);
            var result = await httpClient.GetAsync(url, tokenSource.Token);
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
        public int Rssi { get; set; }
    }

    public class SensorInfo
    {
        public string Name { get; set; }
        public string MACAddress { get; set; }
        public int Rssi { get; set; }
    }

}
