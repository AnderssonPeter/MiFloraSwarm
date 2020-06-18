using System;
using System.Linq;
using EntityGraphQL.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiFloraGateway.Database;
using MiFloraGateway.Devices;
using MiFloraGateway.Plants;
using MiFloraGateway.Sensors;

namespace MiFloraGateway.GraphQL
{
    public class Schema
    {
        public static SchemaProvider<DatabaseContext> MakeSchema()
        {
            // build our schema directly from the DB Context
            var schema = SchemaBuilder.FromObject<DatabaseContext>();

            /*schema.AddScalarType<DateTime>("Date", "");*/
            schema.AddScalarType<DateTimeOffset>("DateTimeOffset", "");
            schema.AddScalarType<TimeSpan>("TimeSpan", "");
            /*schema.AddScalarType<short>("Short", "");*/
            //schema.AddCustomScalarType(typeof(void), "Void");

            //schema.RemoveType<IdentityUser>(); //Users
            schema.RemoveTypeAndAllFields<IdentityRole>(); //Roles
            schema.RemoveTypeAndAllFields<IdentityUserLogin<string>>(); //UserLogins
            schema.RemoveTypeAndAllFields<IdentityRoleClaim<string>>(); //RoleClaims
            schema.RemoveTypeAndAllFields<IdentityUserRole<string>>(); //UserRoles
            schema.RemoveTypeAndAllFields<IdentityUserClaim<string>>(); //UserClaims
            schema.RemoveTypeAndAllFields<IdentityUserToken<string>>(); //UserTokens

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
}
