using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EntityGraphQL.Authorization;
using EntityGraphQL.Schema;
using Microsoft.AspNetCore.Identity;
using MiFloraGateway.Authentication;
using MiFloraGateway.Database;

namespace MiFloraGateway.Devices
{
    public class AddDeviceParameters
    {
        [Required]
        [RegularExpression(ValidationPatterns.MACAddressRegex)]
        public string MACAddress { get; set; } = null!;

        [Required]
        [RegularExpression(ValidationPatterns.IPAddressRegex)]
        public string IPAddress { get; set; } = null!;

        [Required]
        public int Port { get; set; }

        [Required]
        public string Name { get; set; } = null!;
    }

    public class EditDeviceParameters : AddDeviceParameters
    {
        [Required]
        public int Id { get; set; }
    }

    public class DeleteDeviceParameters
    {
        [Required]
        public int Id { get; set; }
    }

    public class DeviceMutations
    {
        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public Expression<Func<DatabaseContext, Device>> AddDevice(DatabaseContext databaseContext, AddDeviceParameters model, Func<Task<IdentityUser>> getUser)
        {
            try
            {
                //get current user, try to convert this into a async method so we can get the user!
                var device = new Device()
                {
                    MACAddress = model.MACAddress,
                    IPAddress = model.IPAddress,
                    Port = model.Port,
                    Name = model.Name
                };
                databaseContext.Devices.Add(device);
                databaseContext.SaveChanges();
                databaseContext.AddLogEntry(LogEntryEvent.Add, device: device).Success();
                return ctx => ctx.Devices.Single(x => x.Id == device.Id);
            }
            catch (Exception ex)
            {
                databaseContext.AddLogEntry(LogEntryEvent.Add).Failure(ex, "Failed to add device!");
                throw ex;
            }
        }

        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public Expression<Func<DatabaseContext, Device>> EditDevice(DatabaseContext databaseContext, EditDeviceParameters model, Func<Task<IdentityUser>> getUser)
        {
            //get current user, try to convert this into a async method so we can get the user!
            var device = databaseContext.Devices.Single(x => x.Id == model.Id);
            using (var logEntry = databaseContext.AddLogEntry(LogEntryEvent.Edit, device: device))
            {
                try
                {
                    device.MACAddress = model.MACAddress;
                    device.IPAddress = model.IPAddress;
                    device.Port = model.Port;
                    device.Name = model.Name;
                    databaseContext.SaveChanges();
                    logEntry.Success();
                    return ctx => ctx.Devices.Single(x => x.Id == device.Id);
                }
                catch (Exception ex)
                {
                    logEntry.Failure(ex, "Failed to edit device!");
                    throw ex;
                }
            }
        }

        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public Empty DeleteDevice(DatabaseContext databaseContext, DeleteDeviceParameters model, Func<Task<IdentityUser>> getUser)
        {
            //get current user, try to convert this into a async method so we can get the user!
            var device = databaseContext.Devices.Single(x => x.Id == model.Id);
            using (var logEntry = databaseContext.AddLogEntry(LogEntryEvent.Delete, device: device))
            {
                try
                {
                    databaseContext.Devices.Remove(device);
                    databaseContext.SaveChanges();
                    logEntry.Success();
                    return Empty.Instance;
                }
                catch (Exception ex)
                {
                    logEntry.Failure(ex, "Failed to delete device!");
                    throw ex;
                }
            }
        }

        /* Wait for EntityGraphQL to support async mutations: https://github.com/lukemurray/EntityGraphQL/issues/50[GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public async Task<Expression<Func<DatabaseContext, IQueryable<Device>>>> Scan(DatabaseContext databaseContext, Func<Task<IdentityUser>> getUser, IJobManager jobManager)
        {
            var ids = await jobManager.StartWaitAsync<IEnumerable<int>, IDetectDeviceCommand>(ddc => ddc.ScanAsync()).ConfigureAwait(false);
            return ctx => ctx.Devices.Where(x => ids.Contains(x.Id));
        }*/
    }
}
