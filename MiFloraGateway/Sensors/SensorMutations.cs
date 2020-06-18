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

namespace MiFloraGateway.Sensors
{

    [MutationArguments]
    public class AddSensorParameters
    {
        [Required]
        [RegularExpression(ValidationPatterns.MACAddressRegex)]
        public string MACAddress { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        public int? PlantId { get; set; }
    }

    public class EditSensorParameters : AddSensorParameters
    {
        [Required]
        public int Id { get; set; }
    }

    [MutationArguments]
    public class DeleteSensorParameters
    {
        [Required]
        public int Id { get; set; }
    }

    public class SensorMutations
    {
        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public async Task<Expression<Func<DatabaseContext, Sensor>>> AddSensor(DatabaseContext databaseContext, AddSensorParameters model, LogEntryHandler logEntryHandler)
        {
            await using (var logEntry = logEntryHandler.AddLogEntry(LogEntryEvent.Add))
            {
                try
                {
                    var plant = model.PlantId.HasValue ? await databaseContext.Plants.GetRequiredByIdAsync(model.PlantId.Value) : null;
                    var sensor = new Sensor()
                    {
                        MACAddress = model.MACAddress,
                        Name = model.Name,
                        Plant = plant
                    };
                    databaseContext.Sensors.Add(sensor);
                    await databaseContext.SaveChangesAsync();
                    logEntry.Attach(sensor);
                    logEntry.Success();
                    return ctx => ctx.Sensors.GetRequiredById(sensor.Id);
                }
                catch (Exception ex)
                {
                    logEntry.Failure(ex, "Failed to add Sensor!");
                    throw;
                }
            }
        }

        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public async Task<Expression<Func<DatabaseContext, Sensor>>> EditSensor(DatabaseContext databaseContext, EditSensorParameters model, LogEntryHandler logEntryHandler)
        {
            var sensor = await databaseContext.Sensors.GetRequiredByIdAsync(model.Id);
            await using (var logEntry = logEntryHandler.AddLogEntry(LogEntryEvent.Edit, sensor: sensor))
            {
                try
                {
                    var plant = model.PlantId.HasValue ? await databaseContext.Plants.GetRequiredByIdAsync(model.PlantId.Value) : null;
                    sensor.MACAddress = model.MACAddress;
                    sensor.Name = model.Name;
                    sensor.Plant = plant;
                    await databaseContext.SaveChangesAsync();
                    logEntry.Success();
                    return ctx => ctx.Sensors.GetRequiredById(sensor.Id);
                }
                catch (Exception ex)
                {
                    logEntry.Failure(ex, "Failed to edit Sensor!");
                    throw;
                }
            }
        }

        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public async Task<Sensor> DeleteSensor(DatabaseContext databaseContext, DeleteSensorParameters model, LogEntryHandler logEntryHandler)
        {
            //get current user, try to convert this into a async method so we can get the user!
            var sensor = await databaseContext.Sensors.GetRequiredByIdAsync(model.Id);
            await using (var logEntry = logEntryHandler.AddLogEntry(LogEntryEvent.Delete, sensor: sensor))
            {
                try
                {
                    databaseContext.Sensors.Remove(sensor);
                    await databaseContext.SaveChangesAsync();
                    logEntry.Success();
                    return sensor;
                }
                catch (Exception ex)
                {
                    logEntry.Failure(ex, "Failed to delete Sensor!");
                    throw;
                }
            }
        }

        
        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public async Task<Expression<Func<DatabaseContext, IEnumerable<Sensor>>>> ScanForDevices(IJobManager jobManager)
        {
            //var ids = await jobManager.StartWaitAsync<IEnumerable<int>, IDetectSensorCommand>(ddc => ddc.ScanAsync()).ConfigureAwait(false);
            return ctx => ctx.Sensors.Where(x => 1 == 1).AsEnumerable(); //GetByIds(ids);
        }
    }
}
