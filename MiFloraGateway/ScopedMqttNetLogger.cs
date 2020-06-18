using System;
using Microsoft.Extensions.Logging;
using MQTTnet.Diagnostics;

namespace MiFloraGateway
{
    public class ScopedMqttNetLogger : IMqttNetScopedLogger
    {
        private readonly ILogger<DataTransmitter> logger;
        private readonly string source;

        public event EventHandler<MqttNetLogMessagePublishedEventArgs>? LogMessagePublished;

        public ScopedMqttNetLogger(ILogger<DataTransmitter> logger, string source)
        {
            this.logger = logger;
            this.source = source;
        }

        public void Publish(MqttNetLogLevel logLevel, string message, object[] parameters, Exception exception)
        {
            var level = GetLogLevel(logLevel);
            logger.Log(level, exception, message, parameters);
            var logMessagePublished = LogMessagePublished;
            if (logMessagePublished != null)
            {
                var logMessage = new MqttNetLogMessage
                {
                    LogId = "DataTransmitter",
                    Timestamp = DateTime.UtcNow,
                    Source = source,
                    ThreadId = Environment.CurrentManagedThreadId,
                    Level = logLevel,
                    Message = message,
                    Exception = exception
                };
                logMessagePublished.Invoke(this, new MqttNetLogMessagePublishedEventArgs(logMessage));
            }
        }

        private LogLevel GetLogLevel(MqttNetLogLevel logLevel)
        {
            switch (logLevel)
            {
                case MqttNetLogLevel.Verbose:
                    return LogLevel.Trace;
                case MqttNetLogLevel.Info:
                    return LogLevel.Information;
                case MqttNetLogLevel.Warning:
                    return LogLevel.Warning;
                case MqttNetLogLevel.Error:
                    return LogLevel.Error;
                default:
                    return LogLevel.Critical;
            }
        }

        public IMqttNetScopedLogger CreateScopedLogger(string source)
        {
            return new ScopedMqttNetLogger(logger, this.source + "." + source);
        }
    }
}
