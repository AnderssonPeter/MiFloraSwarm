using System;
using System.Linq;
using System.Linq.Expressions;
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
            schema.AddScalarType<DateTimeOffset>(nameof(DateTimeOffset), "");
            schema.AddScalarType<TimeSpan>(nameof(TimeSpan), "");
            schema.AddEnum(nameof(SortOrder), typeof(SortOrder), "");
            schema.AddEnum(nameof(DeviceSortField), typeof(DeviceSortField), "");
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

            schema.AddField("devicePager", new { page = 1, pageSize = 10, search = "", orderBy = DeviceSortField.Name, sortOrder = SortOrder.Ascending },
                (db, p) => PaginateDevices(db, p),
                "Pagination. [defaults: page = 1, pageSize = 10, search = \"\", orderBy = \"name\", sortAsc = true]",
                "DevicePagination");

            schema.Type<IdentityUser>().RemoveField(x => x.PasswordHash);

            return schema;
        }

        public static DevicePagination PaginateDevices(DatabaseContext databaseContext, dynamic arg)
        {
            int page = (int)arg.page;
            int pageSize = (int)arg.pageSize;
            string search = (string)arg.search;
            DeviceSortField orderBy = (DeviceSortField)arg.orderBy;
            SortOrder sortOrder = (SortOrder)arg.sortOrder;
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
            Expression<Func<Device, string>> keySelector;
            switch (orderBy)
            {
                case DeviceSortField.MACAddress:
                    keySelector = (Device device) => device.MACAddress;
                    break;
                case DeviceSortField.IPAddress:
                    keySelector = (Device device) => device.IPAddress;
                    break;
                case DeviceSortField.Name:
                    keySelector = (Device device) => device.Name;
                    break;
                default:
                    throw new InvalidOperationException("Unknown sort field!");
            }
            if (sortOrder == SortOrder.Ascending)
            {
                baseQuery = baseQuery.OrderBy(keySelector);
            }
            else
            {
                baseQuery = baseQuery.OrderByDescending(keySelector);
            }

            //Pagination
            int total = baseQuery.Count();
            int pageCount = (int)Math.Ceiling((float)total / (float)pageSize);
            int skipTo = (page * pageSize) - (pageSize);
            baseQuery = baseQuery.Skip(skipTo)
                                 .Take(pageSize)
                                 .AsNoTracking();
            return new DevicePagination
            {
                Devices = baseQuery,
                PageCount = pageCount
            };
        }
    }
}
