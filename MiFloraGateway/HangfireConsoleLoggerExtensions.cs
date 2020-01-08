using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System;
using System.Threading;

namespace MiFloraGateway
{
    public interface ICancellationTokenAccessor
    {
        CancellationToken Get();
    }

    public class CancellationTokenAccessor : ICancellationTokenAccessor
    {
        private readonly IPerformingContextAccessor performingContextAccessor;

        public CancellationTokenAccessor(IPerformingContextAccessor performingContextAccessor)
        {
            this.performingContextAccessor = performingContextAccessor;
        }

        public CancellationToken Get()
        {
            return performingContextAccessor.Get()?.CancellationToken?.ShutdownToken ?? CancellationToken.None;
        }
    }

    public static class HangfireConsoleLoggerExtensions
    {

        public static ILoggingBuilder AddHangfireConsole(this ILoggingBuilder builder)
        {
            builder.AddConfiguration(); 
            builder.Services.AddSingleton<IPerformingContextAccessor, AsyncLocalLogFilter>();
            builder.Services.AddSingleton<ILoggerProvider, HangfireLoggerProvider>();
            builder.Services.AddSingleton<ICancellationTokenAccessor, CancellationTokenAccessor>();
            return builder;
        }
    }
}
