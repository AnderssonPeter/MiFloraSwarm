﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiFloraGateway.Database;

namespace MiFloraGateway.Sensors
{
    public interface ISendValuesCommand
    {
        Task CommandAsync(int sensorId);
    }

    public class SendValuesCommand : ISendValuesCommand
    {
        private readonly IDataTransmitter dataTransmitter;
        private readonly DatabaseContext databaseContext;
        private readonly ILogger<SendValuesCommand> logger;
        private readonly CancellationToken cancellationToken;

        public SendValuesCommand(IDataTransmitter dataTransmitter, DatabaseContext databaseContext,
                                 ILogger<SendValuesCommand> logger, IJobCancellationToken cancellationToken)
        {
            this.dataTransmitter = dataTransmitter;
            this.databaseContext = databaseContext;
            this.logger = logger;
            this.cancellationToken = cancellationToken.ShutdownToken;
        }

        public async Task CommandAsync(int sensorId)
        {
            logger.LogTrace("CommandSync({sensorId})", sensorId);
            //todo: combine inte one query!
            var name = await databaseContext.Sensors.Where(s => s.Id == sensorId).Select(s => s.Name).SingleAsync();
            var dataReadings = await databaseContext.Sensors.Where(s => s.Id == sensorId).Select(s => s.DataReadings.OrderBy(dr => dr.When).First()).SingleAsync();
            var batteryAndFirmware = await databaseContext.Sensors.Where(s => s.Id == sensorId).Select(s => s.BatteryAndVersionReadings.OrderBy(dr => dr.When).First()).SingleAsync();
            await dataTransmitter.SendAsync(name, dataReadings.Brightness, dataReadings.Temperature, dataReadings.Moisture, dataReadings.Conductivity, batteryAndFirmware.Battery, batteryAndFirmware.Version, cancellationToken).ConfigureAwait(false);
        }
    }
}
