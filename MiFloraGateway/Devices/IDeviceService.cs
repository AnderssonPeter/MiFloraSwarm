using MiFlora.Common;
using MiFloraGateway.Database;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway.Devices
{
    public interface IDeviceService
    {
        Task<IEnumerable<SensorInfo>> ScanAsync(IPEndPoint endpoint, CancellationToken cancellationToken = default);
        Task<BatteryAndVersionInfo> GetBatteryAndVersionAsync(IPEndPoint endpoint, string sensorAddress, CancellationToken cancellationToken = default);
        Task<ValuesInfo> GetValuesAsync(IPEndPoint endpoint, string sensorAddress, CancellationToken cancellationToken = default);
        Task<DeviceInfo> GetDeviceInfoAsync(IPEndPoint endpoint, CancellationToken cancellationToken = default);
    }

    public static class DeviceServiceExtensionMethods
    {
        public static Task<IEnumerable<SensorInfo>> ScanAsync(this IDeviceService deviceServices, Device device, CancellationToken cancellationToken = default) =>
            deviceServices.ScanAsync(new IPEndPoint(IPAddress.Parse(device.IPAddress), device.Port), cancellationToken);
        public static Task<BatteryAndVersionInfo> GetBatteryAndVersionAsync(this IDeviceService deviceServices, Device device, string sensorAddress, CancellationToken cancellationToken = default) =>
            deviceServices.GetBatteryAndVersionAsync(new IPEndPoint(IPAddress.Parse(device.IPAddress), device.Port), sensorAddress, cancellationToken);
        public static Task<ValuesInfo> GetValuesAsync(this IDeviceService deviceServices, Device device, string sensorAddress, CancellationToken cancellationToken = default) =>
            deviceServices.GetValuesAsync(new IPEndPoint(IPAddress.Parse(device.IPAddress), device.Port), sensorAddress, cancellationToken);
        public static Task<DeviceInfo> GetDeviceInfoAsync(this IDeviceService deviceServices, Device device, CancellationToken cancellationToken = default) =>
            deviceServices.GetDeviceInfoAsync(new IPEndPoint(IPAddress.Parse(device.IPAddress), device.Port), cancellationToken);

    }
}
