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
using MiFloraGateway.Logs;
using MiFloraGateway.Sensors;

namespace MiFloraGateway.Plants
{

    [MutationArguments]
    public class AddPlantParameters
    {
        [Required]
        public string LatinName { get; set; } = null!;

        [Required]
        public string Alias { get; set; } = null!;

        [Required]
        public string Display { get; set; } = null!;

        [Required]
        public string ImageUrl { get; set; } = null!;

        public string? Blooming { get; set; }
        public string? Category { get; set; }
        public string? Color { get; set; }
        public string? FloralLanguage { get; set; }
        public string? Origin { get; set; }
        public string? Production { get; set; }

        public string? Fertilization { get; set; }
        public string? Pruning { get; set; }
        public string? Size { get; set; }
        public string? Soil { get; set; }
        public string? Sunlight { get; set; }
        public string? Watering { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public int? MinEnvironmentHumidity { get; set; }
        public int? MaxEnvironmentHumidity { get; set; }

        /// <summary>
        /// Lux
        /// </summary>
        public int? MinLightLux { get; set; }
        public int? MaxLightLux { get; set; }

        /// <summary>
        /// ????
        /// </summary>
        public int? MinLightMmol { get; set; }
        public int? MaxLightMmol { get; set; }

        /// <summary>
        /// µS/cm
        /// </summary>
        public int? MinSoilFertility { get; set; }
        public int? MaxSoilFertility { get; set; }

        /// <summary>
        /// %
        /// </summary>
        public int? MinSoilHumidity { get; set; }
        public int? MaxSoilHumidity { get; set; }

        /// <summary>
        /// C°
        /// </summary>
        public int? MinTemperature { get; set; }
        public int? MaxTemperature { get; set; }

    }

    public class EditPlantParameters : AddPlantParameters
    {
        [Required]
        public int Id { get; set; }
    }

    [MutationArguments]
    public class DeletePlantParameters
    {
        [Required]
        public int Id { get; set; }
    }

    public class PlantMutations
    {
        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public async Task<Expression<Func<DatabaseContext, Plant>>> AddPlant(DatabaseContext databaseContext, AddPlantParameters model, LogEntryHandler logEntryHandler)
        {
            await using (var logEntry = logEntryHandler.AddLogEntry(LogEntryEvent.Add))
            {
                try
                {
                    var plant = new Plant()
                    {
                        LatinName = model.LatinName,
                        Alias = model.Alias,
                        Display = model.Display,
                        ImageUrl = model.ImageUrl,
                        Basic = new PlantBasic
                        {
                            Blooming = model.Blooming,
                            Category = model.Category,
                            Color = model.Color,
                            FloralLanguage = model.FloralLanguage,
                            Origin = model.Origin,
                            Production = model.Production
                        },
                        Maintenance = new PlantMaintenance
                        {
                            Fertilization = model.Fertilization,
                            Pruning = model.Pruning,
                            Size = model.Size,
                            Soil = model.Soil,
                            Sunlight = model.Sunlight,
                            Watering = model.Watering
                        },
                        Parameters = new PlantParameters
                        {
                            EnvironmentHumidity = new Database.Range(model.MinEnvironmentHumidity, model.MaxEnvironmentHumidity),
                            LightLux = new Database.Range(model.MinLightLux, model.MaxLightLux),
                            LightMmol = new Database.Range(model.MinLightMmol, model.MaxLightMmol),
                            SoilFertility = new Database.Range(model.MinSoilFertility, model.MaxSoilFertility),
                            SoilHumidity = new Database.Range(model.MinSoilHumidity, model.MaxSoilHumidity),
                            Temperature = new Database.Range(model.MinTemperature, model.MaxTemperature)
                        }
                    };
                    databaseContext.Plants.Add(plant);
                    await databaseContext.SaveChangesAsync();
                    logEntry.Attach(plant);
                    logEntry.Success();
                    return ctx => ctx.Plants.GetRequiredById(plant.Id);
                }
                catch (Exception ex)
                {
                    logEntry.Failure(ex, "Failed to add plant!");
                    throw;
                }
            }
        }

        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public async Task<Expression<Func<DatabaseContext, Plant>>> EditPlant(DatabaseContext databaseContext, EditPlantParameters model, LogEntryHandler logEntryHandler)
        {
            var plant = await databaseContext.Plants.GetRequiredByIdAsync(model.Id);
            await using (var logEntry = logEntryHandler.AddLogEntry(LogEntryEvent.Edit, plant: plant))
            {                
                try
                {
                    plant.LatinName = model.LatinName;
                    plant.Alias = model.Alias;
                    plant.Display = model.Display;
                    plant.ImageUrl = model.ImageUrl;

                    plant.Basic.Blooming = model.Blooming;
                    plant.Basic.Category = model.Category;
                    plant.Basic.Color = model.Color;
                    plant.Basic.FloralLanguage = model.FloralLanguage;
                    plant.Basic.Origin = model.Origin;
                    plant.Basic.Production = model.Production;

                    plant.Maintenance.Fertilization = model.Fertilization;
                    plant.Maintenance.Pruning = model.Pruning;
                    plant.Maintenance.Size = model.Size;
                    plant.Maintenance.Soil = model.Soil;
                    plant.Maintenance.Sunlight = model.Sunlight;
                    plant.Maintenance.Watering = model.Watering;

                    plant.Parameters.EnvironmentHumidity = new Database.Range(model.MinEnvironmentHumidity, model.MaxEnvironmentHumidity);
                    plant.Parameters.LightLux = new Database.Range(model.MinLightLux, model.MaxLightLux);
                    plant.Parameters.LightMmol = new Database.Range(model.MinLightMmol, model.MaxLightMmol);
                    plant.Parameters.SoilFertility = new Database.Range(model.MinSoilFertility, model.MaxSoilFertility);
                    plant.Parameters.SoilHumidity = new Database.Range(model.MinSoilHumidity, model.MaxSoilHumidity);
                    plant.Parameters.Temperature = new Database.Range(model.MinTemperature, model.MaxTemperature);

                    await databaseContext.SaveChangesAsync();
                    logEntry.Success();
                    return ctx => ctx.Plants.GetRequiredById(plant.Id);
                }
                catch (Exception ex)
                {
                    logEntry.Failure(ex, "Failed to edit plant!");
                    throw;
                }
            }
        }

        [GraphQLMutation]
        [GraphQLAuthorize(Roles.Admin)]
        public async Task<Plant> DeletePlant(DatabaseContext databaseContext, DeletePlantParameters model, LogEntryHandler logEntryHandler)
        {
            var plant = await databaseContext.Plants.GetRequiredByIdAsync(model.Id);
            await using (var logEntry = logEntryHandler.AddLogEntry(LogEntryEvent.Delete, plant: plant))
            {
                try
                {
                    databaseContext.Plants.Remove(plant);
                    await databaseContext.SaveChangesAsync();
                    logEntry.Success();
                    return plant;
                }
                catch (Exception ex)
                {
                    logEntry.Failure(ex, "Failed to delete plant!");
                    throw;
                }
            }
        }
    }
}
