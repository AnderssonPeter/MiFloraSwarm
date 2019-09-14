using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway.Devices
{
    public interface IDeviceService
    {
        Task<IEnumerable<SensorInfo>> ScanAsync(string ipAddress, CancellationToken cancellationToken = default);
        Task<BatteryAndVersionInfo> GetBatteryAndVersionAsync(string ipAddress, string sensorAddress, CancellationToken cancellationToken = default);
        Task<ValuesInfo> GetValuesAsync(string ipAddress, string sensorAddress, CancellationToken cancellationToken = default);
        Task<DeviceInfo> GetDeviceInfoAsync(string ipAddress, CancellationToken cancellationToken = default);
    }

}
