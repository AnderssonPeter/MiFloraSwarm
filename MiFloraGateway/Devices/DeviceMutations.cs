using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EntityGraphQL.Authorization;
using EntityGraphQL.Schema;
using Hangfire.Console.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiFloraGateway.Authentication;
using MiFloraGateway.Database;
using MiFloraGateway.Logs;
using MiFloraGateway.Sensors;

namespace MiFloraGateway.Devices
{
    [MutationArguments]
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

    [MutationArguments]
    public class DeleteDeviceParameters
    {
        [Required]
        public int Id { get; set; }
    }

    public class DeviceMutations
    {
        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public async Task<Expression<Func<DatabaseContext, Device>>> AddDevice(DatabaseContext databaseContext, AddDeviceParameters model, LogEntryHandler logEntryHandler)
        {
            await using (var logEntry = logEntryHandler.AddLogEntry(LogEntryEvent.Add))
            {
                try
                {
                    var device = new Device()
                    {
                        MACAddress = model.MACAddress,
                        IPAddress = model.IPAddress,
                        Port = model.Port,
                        Name = model.Name
                    };
                    databaseContext.Devices.Add(device);
                    await databaseContext.SaveChangesAsync();
                    logEntry.Attach(device);
                    logEntry.Success();
                    return ctx => ctx.Devices.GetRequiredById(device.Id);
                }
                catch (Exception ex)
                {
                    logEntry.Failure(ex, "Failed to add device!");
                    throw;
                }
            }
        }

        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public async Task<Expression<Func<DatabaseContext, Device>>> EditDevice(DatabaseContext databaseContext, EditDeviceParameters model, LogEntryHandler logEntryHandler)
        {
            var device = await databaseContext.Devices.GetRequiredByIdAsync(model.Id);
            await using (var logEntry = logEntryHandler.AddLogEntry(LogEntryEvent.Edit, device: device))
            {
                try
                {
                    device.MACAddress = model.MACAddress;
                    device.IPAddress = model.IPAddress;
                    device.Port = model.Port;
                    device.Name = model.Name;
                    await databaseContext.SaveChangesAsync();
                    logEntry.Success();
                    return ctx => ctx.Devices.Single(x => x.Id == device.Id);
                }
                catch (Exception ex)
                {
                    logEntry.Failure(ex, "Failed to edit device!");
                    throw;
                }
            }
        }

        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public async Task<Device> DeleteDevice(DatabaseContext databaseContext, DeleteDeviceParameters model, LogEntryHandler logEntryHandler)
        {
            var device = await databaseContext.Devices.GetRequiredByIdAsync(model.Id);
            await using (var logEntry = logEntryHandler.AddLogEntry(LogEntryEvent.Delete, device: device))
            {
                try
                {
                    databaseContext.Devices.Remove(device);
                    await databaseContext.SaveChangesAsync();
                    logEntry.Success();
                    return device;
                }
                catch (Exception ex)
                {
                    logEntry.Failure(ex, "Failed to delete device!");
                    throw;
                }
            }
        }

        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public async Task<Expression<Func<DatabaseContext, IQueryable<Device>>>> ScanForDevices(IJobManager jobManager)
        {
            var ids = await jobManager.StartWaitAsync<IEnumerable<int>, IDetectDeviceCommand>(ddc => ddc.ScanAsync()).ConfigureAwait(false);
            return ctx => ctx.Devices.GetByIds(ids);
        }
    }
}
