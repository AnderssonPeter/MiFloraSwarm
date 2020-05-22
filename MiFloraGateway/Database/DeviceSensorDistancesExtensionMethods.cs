using System.Linq;

namespace MiFloraGateway.Database
{
    public static class DeviceSensorDistancesExtensionMethods
    {

        //Todo: there must be a better way to do this!  (A partition over would solve this!)
        public static IQueryable<Device> GetDeviceUsagePriority(this DatabaseContext databaseContext, Sensor sensor) =>
            databaseContext.DeviceSensorDistances
                           .Where(dsd => dsd.SensorId == sensor.Id &&
                                         dsd.When == databaseContext.DeviceSensorDistances.Where(dsd2 => dsd2.SensorId == sensor.Id && dsd2.DeviceId == dsd.DeviceId)
                                                                                          .Max(dsd2 => dsd2.When))
                           .Where(dsd => dsd.Rssi.HasValue)
                           .OrderByDescending(dsd => dsd.Rssi)
                           .Select(dsd => dsd.Device);

        public static IQueryable<Sensor> GetLatestSensorsForDevice(this DatabaseContext databaseContext, Device device) =>
           databaseContext.DeviceSensorDistances
                          .Where(dsd => dsd.DeviceId == device.Id &&
                                        dsd.When == databaseContext.DeviceSensorDistances.Where(dsd2 => dsd2.DeviceId == device.Id && dsd2.SensorId == dsd.SensorId)
                                                                                         .Max(dsd2 => dsd2.When))
                          .Where(dsd => dsd.Rssi.HasValue)
                          .Select(dsd => dsd.Sensor);

    }
}
