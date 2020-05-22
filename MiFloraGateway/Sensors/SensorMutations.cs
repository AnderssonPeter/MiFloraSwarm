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

namespace MiFloraGateway.Sensors
{
    public class AddSensorParameters
    {
        [Required]
        [RegularExpression(ValidationPatterns.MACAddressRegex)]
        public string MACAddress { get; set; }

        [Required]
        public string Name { get; set; }

        public int? PlantId { get; set; }
    }

    public class EditSensorParameters : AddSensorParameters
    {
        [Required]
        public int Id { get; set; }
    }

    public class DeleteSensorParameters
    {
        [Required]
        public int Id { get; set; }
    }

    public class SensorMutations
    {
        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public Expression<Func<DatabaseContext, Sensor>> AddSensor(DatabaseContext databaseContext, AddSensorParameters model, Func<Task<IdentityUser>> getUser)
        {
            try
            {
                //get current user, try to convert this into a async method so we can get the user!
                var plant = model.PlantId.HasValue ? databaseContext.Plants.Single(plant => plant.Id == model.PlantId.Value) : null;
                var Sensor = new Sensor()
                {
                    MACAddress = model.MACAddress,
                    Name = model.Name,
                    Plant = plant
                };
                databaseContext.Sensors.Add(Sensor);
                databaseContext.SaveChanges();
                databaseContext.AddLogEntry(LogEntryEvent.Add, sensor: Sensor).Success();
                return ctx => ctx.Sensors.Single(x => x.Id == Sensor.Id);
            }
            catch (Exception ex)
            {
                databaseContext.AddLogEntry(LogEntryEvent.Add).Failure(ex, "Failed to add Sensor!");
                throw ex;
            }
        }

        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public Expression<Func<DatabaseContext, Sensor>> EditSensor(DatabaseContext databaseContext, EditSensorParameters model, Func<Task<IdentityUser>> getUser)
        {
            //get current user, try to convert this into a async method so we can get the user!
            var sensor = databaseContext.Sensors.Single(x => x.Id == model.Id);
            using (var logEntry = databaseContext.AddLogEntry(LogEntryEvent.Edit, sensor: sensor))
            {
                try
                {
                    var plant = model.PlantId.HasValue ? databaseContext.Plants.Single(plant => plant.Id == model.PlantId.Value) : null;
                    sensor.MACAddress = model.MACAddress;
                    sensor.Name = model.Name;
                    sensor.Plant = plant;
                    databaseContext.SaveChanges();
                    logEntry.Success();
                    return ctx => ctx.Sensors.Single(x => x.Id == sensor.Id);
                }
                catch (Exception ex)
                {
                    logEntry.Failure(ex, "Failed to edit Sensor!");
                    throw ex;
                }
            }
        }

        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public Empty DeleteSensor(DatabaseContext databaseContext, DeleteSensorParameters model, Func<Task<IdentityUser>> getUser)
        {
            //get current user, try to convert this into a async method so we can get the user!
            var sensor = databaseContext.Sensors.Single(x => x.Id == model.Id);
            using (var logEntry = databaseContext.AddLogEntry(LogEntryEvent.Delete, sensor: sensor))
            {
                try
                {
                    databaseContext.Sensors.Remove(sensor);
                    databaseContext.SaveChanges();
                    logEntry.Success();
                    return Empty.Instance;
                }
                catch (Exception ex)
                {
                    logEntry.Failure(ex, "Failed to delete Sensor!");
                    throw ex;
                }
            }
        }

        /* Wait for EntityGraphQL to support async mutations: https://github.com/lukemurray/EntityGraphQL/issues/50
        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public async Task<Expression<Func<DatabaseContext, IQueryable<Sensor>>>> Scan(DatabaseContext databaseContext, Func<Task<IdentityUser>> getUser, IJobManager jobManager)
        {
            var ids = await jobManager.StartWaitAsync<IEnumerable<int>, IDetectSensorCommand>(ddc => ddc.ScanAsync()).ConfigureAwait(false);
            return ctx => ctx.Sensors.Where(x => ids.Contains(x.Id));
        }*/
    }
}
