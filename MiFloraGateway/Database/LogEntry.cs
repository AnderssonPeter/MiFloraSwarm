using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiFloraGateway.Database
{
    public class LogEntry
    {
        public int Id { get; set; }
        public int? DeviceId { get; set; }
        public Device Device { get; set; }
        public int? SensorId { get; set; }
        public Sensor Sensor { get; set; }        
        public DateTime When { get; set; }
        public TimeSpan Duration { get; set; }
        public LogEntryEvent Event { get; set; }
        public LogEntryResult Result { get; set; }
        public string Message { get; set; }
    }

    public enum LogEntryEvent
    {
        Scan,
        GetValues,
        GetFirmwareAndBattery,
        Edit
    }

    public enum LogEntryResult
    {
        Failed,
        Successful
    }
}
