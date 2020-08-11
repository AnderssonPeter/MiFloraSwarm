using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiFloraGateway.Database;

namespace MiFloraGateway.Logs
{
    public class LogEntryHandler
    {
        private readonly DatabaseContext databaseContext;
        private readonly ILogger<LogEntryHandler> logEntryHandlerLogger;
        private readonly Func<Task<IdentityUser?>> getUserAsync;

        public LogEntryHandler(DatabaseContext databaseContext, ILogger<LogEntryHandler> logEntryHandlerLogger, Func<Task<IdentityUser?>> getUserAsync)
        {
            this.databaseContext = databaseContext;
            this.logEntryHandlerLogger = logEntryHandlerLogger;
            this.getUserAsync = getUserAsync;
        }

        public LogEntryInstance AddLogEntry(LogEntryEvent @event, Device? device = null, Sensor? sensor = null, Plant? plant = null)
        {
            return new LogEntryInstance(databaseContext, logEntryHandlerLogger, getUserAsync, @event, device: device, sensor: sensor, plant: plant);
        }
    }


    public class LogEntryInstance : IAsyncDisposable
    {
        private readonly ILogger<LogEntryHandler> logger;
        private readonly Func<Task<IdentityUser?>> getUserAsync;
        private readonly DatabaseContext databaseContext;
        private readonly LogEntryEvent @event;
        private Device? device;
        private Sensor? sensor;
        private Plant? plant;
        private readonly DateTime when = DateTime.Now;
        private LogEntry? logEntry;


        public LogEntryInstance(DatabaseContext databaseContext, ILogger<LogEntryHandler> logger, Func<Task<IdentityUser?>> getUserAsync, LogEntryEvent @event, Device? device = null, Sensor? sensor = null, Plant? plant = null)
        {
            this.logger = logger;
            this.getUserAsync = getUserAsync;
            this.databaseContext = databaseContext;
            this.@event = @event;
            this.device = device;
            this.sensor = sensor;
            this.plant = plant;
        }

        public void Attach(Device device)
        {
            this.device = device;
        }

        public void Attach(Sensor sensor)
        {
            this.sensor = sensor;
        }

        public void Attach(Plant plant)
        {
            this.plant = plant;
        }
        public void Success(string? message = null) => Save(LogEntryResult.Successful, message);

        public void Failure(string message) => Save(LogEntryResult.Failed, message);

        public void Failure(Exception ex, string? message = null) => Save(LogEntryResult.Failed, ex.ToString() + Environment.NewLine + message);

        private void Save(LogEntryResult result, string? message)
        {
            if (logEntry != null)
            {
                throw new InvalidOperationException("LogEntry can only have one value!");
            }
            logEntry = new LogEntry
            {
                Device = device,
                Sensor = sensor,
                Plant = plant,
                When = when,
                Duration = DateTime.Now.Subtract(when),
                Event = @event,
                Result = result,
                Message = message.Truncate(200)
            };
        }

        public async ValueTask DisposeAsync()
        {
            if (logEntry == null)
            {
                throw new InvalidOperationException("Failed to save LogEntry!");
            }
            if (databaseContext.ChangeTracker.HasChanges())
            {
                if (logEntry.Result == LogEntryResult.Successful)
                {
                    throw new InvalidOperationException("There should be no unsaved changes when it was considered successful!");
                }

                var changedEntriesCopy = databaseContext.ChangeTracker.Entries()
                                                        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

                foreach (var entry in changedEntriesCopy)
                {
                    logger.LogDebug("Removing {entity} from change tracking", entry);
                    entry.State = EntityState.Detached;
                }
            }
            logEntry.User = await getUserAsync();
            databaseContext.LogEntries.Add(logEntry);
            await databaseContext.SaveChangesAsync();
        }
    }

}
