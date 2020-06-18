using System;
using Microsoft.Extensions.Logging;
using MQTTnet.Diagnostics;

namespace MiFloraGateway
{
    public class MqttNetLogger : IMqttNetLogger
    {
        private readonly ILogger<DataTransmitter> logger;

        public event EventHandler<MqttNetLogMessagePublishedEventArgs> LogMessagePublished;

        public MqttNetLogger(ILogger<DataTransmitter> logger)
        {
            this.logger = logger;
        }

        public void Publish(MqttNetLogLevel logLevel, string source, string message, object[] parameters, Exception exception)
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
            return new ScopedMqttNetLogger(logger, source);
        }
    }
}
