//using Hangfire.Console;
//using Hangfire.Server;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace MiFloraGateway
//{
//    public class HangfireConsoleLogger : ILogger
//    {
//        private readonly PerformContext context;
//        private readonly ILogger innerLogger;

//        public HangfireConsoleLogger(PerformContext context, ILogger innerLogger)
//        {
//            this.context = context;
//            this.innerLogger = innerLogger;
//        }

//        public IDisposable BeginScope<TState>(TState state)
//        {
//            return innerLogger.BeginScope(state);
//        }

//        public bool IsEnabled(LogLevel logLevel)
//        {
//            return true;
//        }

//        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
//        {
//            innerLogger.Log(logLevel, eventId, state, exception, formatter);
//            var content = formatter(state, exception);
//            ConsoleTextColor color;
//            switch (logLevel)
//            {
//                case LogLevel.Trace:
//                    color = ConsoleTextColor.DarkGray;
//                    break;
//                case LogLevel.Debug:
//                    color = ConsoleTextColor.Gray;
//                    break;
//                case LogLevel.Information:
//                    color = ConsoleTextColor.White;
//                    break;
//                case LogLevel.Warning:
//                    color = ConsoleTextColor.DarkYellow;
//                    break;
//                case LogLevel.Error:
//                    color = ConsoleTextColor.DarkRed;
//                    break;
//                case LogLevel.Critical:
//                    color = ConsoleTextColor.Red;
//                    break;
//                case LogLevel.None:
//                    color = ConsoleTextColor.White;
//                    break;
//                default:
//                    color = ConsoleTextColor.White;
//                    break;
//            }
            
//            context.WriteLine(color, content);
//        }
//    }
//}
