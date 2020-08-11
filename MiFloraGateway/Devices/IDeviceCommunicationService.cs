using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MiFlora.Common;
using MiFloraGateway.Database;

namespace MiFloraGateway.Devices
{
    public interface IDeviceCommunicationService
    {
        Task<IEnumerable<SensorInfo>> ScanAsync(IPEndPoint endpoint, CancellationToken cancellationToken = default);
        Task<BatteryAndVersionInfo> GetBatteryAndVersionAsync(IPEndPoint endpoint, string sensorAddress, CancellationToken cancellationToken = default);
        Task<ValuesInfo> GetValuesAsync(IPEndPoint endpoint, string sensorAddress, CancellationToken cancellationToken = default);
        Task<DeviceInfo> GetDeviceInfoAsync(IPEndPoint endpoint, CancellationToken cancellationToken = default);
    }

    public static class DeviceCommunicationServiceExtensionMethods
    {
        public static Task<IEnumerable<SensorInfo>> ScanAsync(this IDeviceCommunicationService deviceCommunicationService, Device device, CancellationToken cancellationToken = default) =>
            deviceCommunicationService.ScanAsync(new IPEndPoint(IPAddress.Parse(device.IPAddress), device.Port), cancellationToken);
        public static Task<BatteryAndVersionInfo> GetBatteryAndVersionAsync(this IDeviceCommunicationService deviceCommunicationService, Device device, string sensorAddress, CancellationToken cancellationToken = default) =>
            deviceCommunicationService.GetBatteryAndVersionAsync(new IPEndPoint(IPAddress.Parse(device.IPAddress), device.Port), sensorAddress, cancellationToken);
        public static Task<ValuesInfo> GetValuesAsync(this IDeviceCommunicationService deviceCommunicationService, Device device, string sensorAddress, CancellationToken cancellationToken = default) =>
            deviceCommunicationService.GetValuesAsync(new IPEndPoint(IPAddress.Parse(device.IPAddress), device.Port), sensorAddress, cancellationToken);
        public static Task<DeviceInfo> GetDeviceInfoAsync(this IDeviceCommunicationService deviceCommunicationService, Device device, CancellationToken cancellationToken = default) =>
            deviceCommunicationService.GetDeviceInfoAsync(new IPEndPoint(IPAddress.Parse(device.IPAddress), device.Port), cancellationToken);

    }
}
