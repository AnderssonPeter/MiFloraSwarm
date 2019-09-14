using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiFloraGateway.Database;
using Microsoft.AspNet.OData.Builder;
using Microsoft.OData.Edm;

namespace MiFloraGateway
{
    public class ODataModelBuilder
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EnableLowerCamelCase();

            var sensorEntitySet = builder.EntitySet<Sensor>("Sensors");
            var sensorEntityType = sensorEntitySet.EntityType;
            sensorEntityType.HasMany(x => x.DeviceDistances);
            sensorEntityType.HasMany(x => x.BatteryAndVersionReadings);
            sensorEntityType.HasMany(x => x.DataReadings);
            sensorEntityType.HasMany(x => x.Tags);

            var deviceEntitySet = builder.EntitySet<Device>("Devices");
            var deviceEntityType = deviceEntitySet.EntityType;
            deviceEntityType.HasMany(x => x.SensorDistances);
            deviceEntityType.HasMany(x => x.Tags);

            deviceEntityType.Collection.Function("Scan").ReturnsCollection<Device>();


            var plantEntitySet = builder.EntitySet<Plant>("Plants");
            var plantEntityType = plantEntitySet.EntityType;
            plantEntityType.HasRequired(x => x.Basic);
            plantEntityType.HasRequired(x => x.Maintenance);
            plantEntityType.HasRequired(x => x.Parameters);

            builder.EntityType<DeviceSensorDistance>().HasKey(x => new { x.DeviceId, x.SensorId });
            builder.EntityType<SensorBatteryAndVersionReading>().HasKey(x => new { x.SensorId, x.When });
            builder.EntityType<SensorDataReading>().HasKey(x => new { x.SensorId, x.When });
            builder.EntityType<SensorTag>().HasKey(x => new { x.SensorId, x.Tag });

            builder.EntityType<DeviceTag>().HasKey(x => new { x.DeviceId, x.Tag });

            /*builder.EntityType<SensorTag>().Ignore(s => s.SensorId);
            builder.EntityType<SensorTag>().Ignore(s => s.Sensor);
            builder.EntityType<DeviceTag>().Ignore(s => s.DeviceId);
            builder.EntityType<DeviceTag>().Ignore(s => s.Device);*/

            return builder.GetEdmModel();
        }
    }
}
