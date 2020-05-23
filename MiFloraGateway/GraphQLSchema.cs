using System;
using System.ComponentModel;
using System.Linq;
using EntityGraphQL.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiFloraGateway.Database;
using MiFloraGateway.Devices;
using MiFloraGateway.Plants;
using MiFloraGateway.Sensors;

namespace MiFloraGateway
{

    public class DeviceError
    {
        public DateTime When { get; set; }
        public string? Message { get; set; }
        public DeviceError(DateTime when, string? message)
        {
            this.When = when;
            this.Message = message;
        }
    }
    public class GraphQLSchema
    {
        public static SchemaProvider<DatabaseContext> MakeSchema()
        {
            // build our schema directly from the DB Context
            var schema = SchemaBuilder.FromObject<DatabaseContext>();
            schema.AddCustomScalarType(typeof(DateTime), "Date", true);
            schema.AddCustomScalarType(typeof(DateTime?), "Date");
            schema.AddCustomScalarType(typeof(DateTimeOffset), "DateTimeOffset", true);
            schema.AddCustomScalarType(typeof(DateTimeOffset?), "DateTimeOffset");
            schema.AddCustomScalarType(typeof(TimeSpan), "TimeSpan", true);
            schema.AddCustomScalarType(typeof(TimeSpan?), "TimeSpan");
            schema.AddCustomScalarType(typeof(short), "Short", true);
            schema.AddCustomScalarType(typeof(short?), "Short");
            //schema.AddCustomScalarType(typeof(void), "Void");
            schema.AddType<Empty>("Empty").AddAllFields();
            schema.AddType<DeviceError>("DeviceError").AddAllFields();

            schema.Type<Device>().RemoveField(x => x.IPAddress);
            schema.Type<Device>().AddField("ipAddress", device => device.IPAddress, "");
            schema.Type<Device>().RemoveField(device => device.MACAddress);
            schema.Type<Device>().AddField("macAddress", device => device.MACAddress, "");
            schema.Type<Device>().AddField("failuresLast24Hours", device => device.Logs.Where(log => log.Result == LogEntryResult.Failed).Where(log => log.When >= DateTime.Now.AddDays(-1)).Select(log => new DeviceError(log.When, log.Message)), "Contains all the failure times over the last 24h");

            schema.AddType<AddDeviceParameters>("").AddAllFields();
            schema.AddType<EditDeviceParameters>("").AddAllFields();
            schema.AddType<DeleteDeviceParameters>("").AddAllFields();
            schema.AddMutationFrom(new DeviceMutations());
            schema.AddType<AddSensorParameters>("").AddAllFields();
            schema.AddType<EditSensorParameters>("").AddAllFields();
            schema.AddType<DeleteSensorParameters>("").AddAllFields();
            schema.AddMutationFrom(new SensorMutations());
            schema.AddType<AddPlantParameters>("").AddAllFields();
            schema.AddType<EditPlantParameters>("").AddAllFields();
            schema.AddType<DeletePlantParameters>("").AddAllFields();
            schema.AddMutationFrom(new PlantMutations());

            schema.AddType<DevicePagination>(nameof(DevicePagination), "Device Pagination")
                  .AddAllFields();

            schema.AddField("devicePager", new { page = 1, pageSize = 10, search = "", orderBy = nameof(Device.Name) },
                (db, p) => PaginateDevices(db, p),
                "Pagination. [defaults: page = 1, pageSize = 10, orderBy = \"name\"]",
                "DevicePagination");

            schema.Type<IdentityUser>().RemoveField(x => x.PasswordHash);

            return schema;
        }

        public static DevicePagination PaginateDevices(DatabaseContext databaseContext, dynamic arg)
        {
            int page = (int)arg.page;
            int pageSize = (int)arg.pageSize;
            string search = (string)arg.search;
            IQueryable<Device> baseQuery;
            if (!string.IsNullOrEmpty(search))
            {
                var words = search.Split(' ');
                Func<string, bool> condition = (text) => EF.Functions.Like(text, "%" + search + "%");

                baseQuery = databaseContext.Devices.Where(x => EF.Functions.Like(x.Name, "%" + search + "%") ||
                                                               EF.Functions.Like(x.IPAddress, "%" + search + "%") ||
                                                               EF.Functions.Like(x.MACAddress, "%" + search + "%") ||
                                                               x.Tags.Any(x => EF.Functions.Like(x.Tag, "%" + search + "%") ||
                                                                               EF.Functions.Like(x.Value, "%" + search + "%")));
            }
            else
            {
                baseQuery = databaseContext.Devices;
            }
            //Pagination
            int total = baseQuery.Count();
            int pageCount = ((total + pageSize) / pageSize);
            int skipTo = (page * pageSize) - (pageSize);
            baseQuery = baseQuery.OrderBy(x => x.Name)
                                 .Skip(skipTo)
                                 .Take(pageSize)
                                 .AsNoTracking();
            return new DevicePagination
            {
                Devices = baseQuery,
                PageCount = pageCount,
                Total = total
            };
        }
    }

    public class DevicePagination : Pagination
    {
        [Description("collection of devices")]
        public IQueryable<Device> Devices { get; set; } = null!;
    }

    public class Pagination
    {
        [Description("total records to match search")]
        public int Total { get; set; }
        [Description("total pages based on page size")]
        public int PageCount { get; set; }
    }
}
